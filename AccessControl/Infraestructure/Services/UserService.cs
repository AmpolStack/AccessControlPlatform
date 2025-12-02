using AccessControl.Core.Interfaces.Services;
using AccessControl.Core.Models;
using AccessControl.Infraestructure.Dto;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AccessControl.Infraestructure.Services
{
    public class UserService(AccessControlContext context) : IUserService
    {
        private readonly AccessControlContext _context = context;

        // Logic embedded in sp
        public async Task<(bool Success, string Message, User? User)> LoginAsync(string email, string password)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var pEmail = new SqlParameter("@Email", email);
                var pPassword = new SqlParameter("@Password", password);

                var result = await _context.LoginResultDto
                    .FromSqlRaw("EXEC sp_UserLogin @Email, @Password", pEmail, pPassword)
                    .ToListAsync();

                var dto = result.FirstOrDefault();
                if (dto == null)
                {
                    await transaction.RollbackAsync();
                    return (false, "Invalid response", null);
                }

                if (!dto.Success)
                {
                    await transaction.RollbackAsync();
                    return (false, dto.Message!, null);
                }

                // TODO: Map DTO to User entity with Automapper
                var user = new User
                {
                    Id = dto.Id,
                    Email = dto.Email!,
                    FullName = dto!.FullName!,
                    IdentityDocument = dto.IdentityDocument!,
                    PhoneNumber = dto.PhoneNumber,
                    Role = dto.Role!,
                    EstablishmentId = dto.EstablishmentId,
                    IsActive = dto.IsActive,
                    MustChangePassword = dto.MustChangePassword
                };

                await transaction.CommitAsync();
                return (true, dto.Message!, user);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, ex.Message, null);
            }
        }

        // Embedded logic in stored procedure
        public async Task<(bool Success, string Message)> CreateUserAsync(User user)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var pResult = new SqlParameter("@Result", SqlDbType.Bit)
                {
                    Direction = ParameterDirection.Output
                };

                var pMessage = new SqlParameter("@Message", SqlDbType.NVarChar, 200)
                {
                    Direction = ParameterDirection.Output
                };

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
                    new SqlParameter("@Email", user.Email),
                    new SqlParameter("@Password", user.Password),
                    new SqlParameter("@FullName", user.FullName),
                    new SqlParameter("@IdentityDocument", user.IdentityDocument),
                    new SqlParameter("@PhoneNumber", (object?)user.PhoneNumber ?? DBNull.Value),
                    new SqlParameter("@EstablishmentId", user.EstablishmentId),
                    new SqlParameter("@Role", user.Role),
                    new SqlParameter("@CreatedBy", 0),
                    pResult,
                    pMessage
                );

                var result = new CreateUserResultDto()
                {
                    Success = (bool)pResult.Value,
                    Message = (string)pMessage.Value
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
        public async Task<(bool Success, string Message)> UpdateUserAsync(User user)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var entity = await _context.Users.FindAsync(user.Id);

                if (entity == null)
                {
                    await transaction.RollbackAsync();
                    return (false, "User not found");
                }

                entity.Email = user.Email;
                entity.FullName = user.FullName;
                entity.PhoneNumber = user.PhoneNumber;
                entity.Role = user.Role;
                entity.IdentityDocument = user.IdentityDocument;
                entity.IsActive = user.IsActive;
                entity.EstablishmentId = user.EstablishmentId;

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
