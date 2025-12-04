using AccessControl.Domain.Models;
using AccessControl.Domain.Services;
using AccessControl.Domain.UseCases;
using AccessControl.Infraestructure.Dto;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AccessControl.Infraestructure.UseCases
{
    public class UserUseCases(AccessControlContext context, ITransactionParameterHelpers helpers, IHashingService hashingService) : IUserUseCases
    {
        private readonly AccessControlContext _context = context;
        private readonly ITransactionParameterHelpers _helpers = helpers;
        private readonly IHashingService _hashingService = hashingService;

        public async Task<(bool Success, string Message, UserLoginDataDto? User)> LoginAsync(string email, string password)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Establishment)
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    return (false, "Usuario no encontrado", null);
                }

                if (user.IsActive != true)
                {
                    return (false, "Usuario inactivo", null);
                }


                if (!_hashingService.Check(user.Password, password))
                {
                    return (false, "Credenciales inválidas", null);
                }

                var hasOpenSession = await _context.AccessRecords
                    .AnyAsync(ar => ar.UserId == user.Id && ar.ExitDateTime == null);

                if (hasOpenSession)
                {
                    return (false, "Hay una sesión abierta en este perfil", null);
                }

                user.LastLoginDate = DateTime.Now;
                await _context.SaveChangesAsync();

                var dto = new UserLoginDataDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    EstablishmentId = user.EstablishmentId,
                    Role = user.Role,
                    IsActive = user.IsActive ?? false,
                    IdentityDocument = user.IdentityDocument,
                    PhoneNumber = user.PhoneNumber,
                    MustChangePassword = user.MustChangePassword ?? false,
                    CreatedDate = user.CreatedDate,
                    LastLoginDate = user.LastLoginDate,
                    EstablishmentName = user.Establishment?.Name ?? string.Empty,
                    MaxCapacity = user.Establishment?.MaxCapacity
                };

                return (true, "Login exitoso", dto);
            }
            catch (Exception ex)
            {
                return (false, $"Error en login: {ex.Message}", null);
            }
        }

        // Embedded logic in stored procedure
        public async Task<(bool Success, string Message)> CreateUserAsync(User user)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var pResult = _helpers.CreateOutput("@Result", SqlDbType.Bit);
                var pMessage = _helpers.CreateOutput("@Message", SqlDbType.NVarChar, 200);

                await _context.Database.ExecuteSqlRawAsync(
                    @"EXEC sp_CreateUser 
                        @Email,
                        @Password,
                        @FullName,
                        @IdentityDocument,
                        @PhoneNumber,
                        @EstablishmentId,
                        @Role,
                        @CreatedBy,
                        @Result OUTPUT,
                        @Message OUTPUT",
                    _helpers.CreateInput("@Email", user.Email),
                    _helpers.CreateInput("@Password", user.Password),
                    _helpers.CreateInput("@FullName", user.FullName),
                    _helpers.CreateInput("@IdentityDocument", user.IdentityDocument),
                    _helpers.CreateNullable("@PhoneNumber", user.PhoneNumber),
                    _helpers.CreateInput("@EstablishmentId", user.EstablishmentId),
                    _helpers.CreateInput("@Role", user.Role),
                    _helpers.CreateInput("@CreatedBy", 0),
                    pResult,
                    pMessage
                );

                var result = new CreateUserResultDto
                {
                    Success = pResult.Value != DBNull.Value && Convert.ToBoolean(pResult.Value),
                    Message = pMessage.Value != DBNull.Value ? (string)pMessage.Value : string.Empty
                };

                if (!result.Success)
                {
                    await transaction.RollbackAsync();
                    return (false, result.Message);
                }

                await transaction.CommitAsync();
                return (true, result.Message);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, ex.Message);
            }
        }

        // Implements logic directly, no stored procedure
        public async Task<(bool Success, string Message)> UpdatePropertiesAsync(string email,
            string phone, string identityDocument, string fullname)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var entity = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

                if (entity == null)
                {
                    await transaction.RollbackAsync();
                    return (false, "User not found");
                }

                entity.Email = email;
                entity.FullName = fullname;
                entity.IdentityDocument = identityDocument;
                entity.PhoneNumber = phone;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, "User updated successfully");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, ex.Message);
            }
        }

        // Implements logic directly, no stored procedure
        public async Task<(bool Success, string Message)> DeleteUserAsync(int id)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    await transaction.RollbackAsync();
                    return (false, "User not found");
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return (true, "User deleted");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, ex.Message);
            }
        }

        // Implements logic directly, no stored procedure, and not use transaction
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .ToListAsync();
        }

    }

}
