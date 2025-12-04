using AccessControl.Domain.Models;
using AccessControl.Domain.Services;
using AccessControl.Domain.UseCases;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AccessControl.Infraestructure.UseCases
{
    public class EstablishmentUseCase(AccessControlContext context, ITransactionParameterHelpers helpers) : IEstablishmentUseCase
    {
        private readonly AccessControlContext _context = context;
        private readonly ITransactionParameterHelpers _helpers = helpers;


        public async Task<(bool Success, string Message, int EstablishmentId)> CreateEstablishmentAsync(Establishment establishment, User? user = null)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.Establishments.AddAsync(establishment);
                await _context.SaveChangesAsync();

                if (user != null)
                {
                    user.EstablishmentId = establishment.Id;
                    await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return (true, "Success", establishment.Id);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, ex.InnerException?.Message ?? ex.Message, 0);
            }
        }

        public async Task<(bool Success, string Message)> OpenEstablishmentAsync(int establishmentId, int userId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var pEstablishmentId = _helpers.CreateInput("@EstablishmentId", establishmentId);
                var pUserId = _helpers.CreateInput("@UserId", userId);
                var pResult = _helpers.CreateOutput("@Result", SqlDbType.Bit);
                var pMessage = _helpers.CreateOutput("@Message", SqlDbType.NVarChar, 200);

                await _context.Database.ExecuteSqlRawAsync(
                    @"EXEC sp_OpenEstablishment 
                        @EstablishmentId, 
                        @UserId, 
                        @Result OUTPUT, 
                        @Message OUTPUT",
                    pEstablishmentId,
                    pUserId,
                    pResult,
                    pMessage
                );

                var success = pResult.Value != DBNull.Value && Convert.ToBoolean(pResult.Value);
                var message = pMessage.Value != DBNull.Value ? (string)pMessage.Value : string.Empty;

                if (!success)
                {
                    await transaction.RollbackAsync();
                    return (false, message);
                }

                await transaction.CommitAsync();
                return (true, message);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string Message)> CloseEstablishmentAsync(int establishmentId, int userId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var pEstablishmentId = _helpers.CreateInput("@EstablishmentId", establishmentId);
                var pUserId = _helpers.CreateInput("@UserId", userId);
                var pResult = _helpers.CreateOutput("@Result", SqlDbType.Bit);
                var pMessage = _helpers.CreateOutput("@Message", SqlDbType.NVarChar, 200);

                await _context.Database.ExecuteSqlRawAsync(
                    @"EXEC sp_CloseEstablishment 
                        @EstablishmentId, 
                        @UserId, 
                        @Result OUTPUT, 
                        @Message OUTPUT",
                    pEstablishmentId,
                    pUserId,
                    pResult,
                    pMessage
                );

                var success = pResult.Value != DBNull.Value && Convert.ToBoolean(pResult.Value);
                var message = pMessage.Value != DBNull.Value ? (string)pMessage.Value : string.Empty;

                if (!success)
                {
                    await transaction.RollbackAsync();
                    return (false, message);
                }

                await transaction.CommitAsync();
                return (true, message);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string Message, Establishment? Establishment)> GetEstablishmentAsync(int establishmentId)
        {
            try
            {
                var resp = await _context.Establishments.FindAsync(establishmentId);
                if (resp == null) return (false, "No se encontr� establecimiento relacionado con el id", null);

                return (true, "Establecimiento encontrado", resp);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public async Task<(bool Success, string Message, int EstablishmentId)> UpdateEstablishmentAsync(Establishment establishment)
        {
            return await CreateEstablishmentAsync(establishment, null);
        }
    }
}

