using AccessControl.Core.Interfaces.Common;
using AccessControl.Core.Interfaces.Services;
using AccessControl.Core.Models;
using AccessControl.Infraestructure.Dto;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AccessControl.Infraestructure.Services
{
    public class AccessRecordService(AccessControlContext context, ITransactionParameterHelpers helpers) : IAccessRecordService
    {
        private readonly AccessControlContext _context = context;
        private readonly ITransactionParameterHelpers _helpers = helpers;

        public async Task<(bool Success, string Message)> RegisterEntryAsync(string identityDocument, int establishmentId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var pIdentityDocument = _helpers.CreateInput("@IdentityDocument", identityDocument);
                var pEstablishmentId = _helpers.CreateInput("@EstablishmentId", establishmentId);
                var pResult = _helpers.CreateOutput("@Result", SqlDbType.Bit);
                var pMessage = _helpers.CreateOutput("@Message", SqlDbType.NVarChar, 200);

                await _context.Database.ExecuteSqlRawAsync(
                    @"EXEC sp_RegisterEntry 
                        @IdentityDocument, 
                        @EstablishmentId, 
                        @Result OUTPUT, 
                        @Message OUTPUT",
                    pIdentityDocument,
                    pEstablishmentId,
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

        public async Task<(bool Success, string Message)> RegisterExitAsync(string identityDocument, int establishmentId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var pIdentityDocument = _helpers.CreateInput("@IdentityDocument", identityDocument);
                var pEstablishmentId = _helpers.CreateInput("@EstablishmentId", establishmentId);
                var pResult = _helpers.CreateOutput("@Result", SqlDbType.Bit);
                var pMessage = _helpers.CreateOutput("@Message", SqlDbType.NVarChar, 200);

                await _context.Database.ExecuteSqlRawAsync(
                    @"EXEC sp_RegisterExit 
                        @IdentityDocument, 
                        @EstablishmentId, 
                        @Result OUTPUT, 
                        @Message OUTPUT",
                    pIdentityDocument,
                    pEstablishmentId,
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

        public async Task<(bool Success, string Message, CurrentCapacityDto? Data)> GetCurrentCapacityAsync(int establishmentId)
        {
            try
            {
                var pEstablishmentId = _helpers.CreateInput("@EstablishmentId", establishmentId);
                var pSuccess = _helpers.CreateOutput("@Success", SqlDbType.Bit);
                var pMessage = _helpers.CreateOutput("@Message", SqlDbType.NVarChar, 200);

                var result = await _context.CurrentCapacityDto
                    .FromSqlRaw(@"EXEC sp_GetCurrentCapacity @EstablishmentId, @Success OUTPUT, @Message OUTPUT",
                        pEstablishmentId, pSuccess, pMessage)
                    .AsNoTracking()
                    .ToListAsync();

                var success = (bool)pSuccess.Value;
                var message = (string)pMessage.Value;

                if (!success)
                {
                    return (false, message, null);
                }

                var data = result.FirstOrDefault();
                return (true, message, data);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public async Task<(bool Success, string Message, IEnumerable<UserAccessHistoryDto>? Data)> GetAccessHistoryAsync(
            int establishmentId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var pEstablishmentId = _helpers.CreateInput("@EstablishmentId", establishmentId);
                var pStartDate = _helpers.CreateInput("@StartDate", startDate);
                var pEndDate = _helpers.CreateInput("@EndDate", endDate);
                var pSuccess = _helpers.CreateOutput("@Success", SqlDbType.Bit);
                var pMessage = _helpers.CreateOutput("@Message", SqlDbType.NVarChar, 200);

                var result = await _context.UserAccessHistoryDto
                    .FromSqlRaw(@"EXEC sp_GetUserAccessHistory 
                        @EstablishmentId, @StartDate, @EndDate, @Success OUTPUT, @Message OUTPUT",
                        pEstablishmentId, pStartDate, pEndDate, pSuccess, pMessage)
                    .AsNoTracking()
                    .ToListAsync();

                var success = (bool)pSuccess.Value;
                var message = (string)pMessage.Value;

                if (!success)
                {
                    return (false, message, null);
                }

                return (true, message, result);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public async Task<(bool Success, string Message, IEnumerable<HourlyAverageDto>? Data)> GetHourlyAveragesAsync(
            int establishmentId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var pEstablishmentId = _helpers.CreateInput("@EstablishmentId", establishmentId);
                var pStartDate = _helpers.CreateInput("@StartDate", startDate);
                var pEndDate = _helpers.CreateInput("@EndDate", endDate);
                var pSuccess = _helpers.CreateOutput("@Success", SqlDbType.Bit);
                var pMessage = _helpers.CreateOutput("@Message", SqlDbType.NVarChar, 200);

                var result = await _context.HourlyAverageDto
                    .FromSqlRaw(@"EXEC sp_GetHourlyAverages 
                        @EstablishmentId, @StartDate, @EndDate, @Success OUTPUT, @Message OUTPUT",
                        pEstablishmentId, pStartDate, pEndDate, pSuccess, pMessage)
                    .AsNoTracking()
                    .ToListAsync();

                var success = (bool)pSuccess.Value;
                var message = (string)pMessage.Value;

                if (!success)
                {
                    return (false, message, null);
                }

                return (true, message, result);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
    }
}
