using AccessControl.Domain.Models;
using AccessControl.Domain.Services;
using AccessControl.Domain.UseCases;
using AccessControl.Infraestructure.Dto;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AccessControl.Infraestructure.UseCases
{
    public class AccessRecordUseCase(AccessControlContext context, ITransactionParameterHelpers helpers) : IAccessRecordUseCase
    {
        private readonly AccessControlContext _context = context;
        private readonly ITransactionParameterHelpers _helpers = helpers;

        public async Task<(bool Success, string Message)> RegisterEntryAsync(int userId, int establishmentId)
        {
            try
            {
                var pIdentityDocument = _helpers.CreateInput("@UserId", userId);
                var pEstablishmentId = _helpers.CreateInput("@EstablishmentId", establishmentId);
                var pResult = _helpers.CreateOutput("@Result", SqlDbType.Bit);
                var pMessage = _helpers.CreateOutput("@Message", SqlDbType.NVarChar, 200);

                await _context.Database.ExecuteSqlRawAsync(
                    @"EXEC sp_RegisterEntry 
                        @UserId, 
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

                return (success, message);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string Message)> RegisterExitAsync(int userId, int establishmentId)
        {
            try
            {
                var pIdentityDocument = _helpers.CreateInput("@UserId", userId);
                var pEstablishmentId = _helpers.CreateInput("@EstablishmentId", establishmentId);
                var pResult = _helpers.CreateOutput("@Result", SqlDbType.Bit);
                var pMessage = _helpers.CreateOutput("@Message", SqlDbType.NVarChar, 200);

                await _context.Database.ExecuteSqlRawAsync(
                    @"EXEC sp_RegisterExit 
                        @UserId, 
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

                return (success, message);
            }
            catch (Exception ex)
            {
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
