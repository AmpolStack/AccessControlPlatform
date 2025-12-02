using AccessControl.Core.Interfaces.Common;
using AccessControl.Core.Interfaces.Services;
using AccessControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AccessControl.Infraestructure.Services
{
    public class EstablishmentService(AccessControlContext context, ITransactionParameterHelpers helpers) : IEstablishmentService
    {
        private readonly AccessControlContext _context = context;
        private readonly ITransactionParameterHelpers _helpers = helpers;

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

                var success = (bool)pResult.Value;
                var message = (string)pMessage.Value;

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

                var success = (bool)pResult.Value;
                var message = (string)pMessage.Value;

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
    }
}
