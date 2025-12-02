using AccessControl.Infraestructure.Dto;

namespace AccessControl.Core.Interfaces.Services
{
    public interface IAccessRecordService
    {
        Task<(bool Success, string Message)> RegisterEntryAsync(string identityDocument, int establishmentId);

        Task<(bool Success, string Message)> RegisterExitAsync(string identityDocument, int establishmentId);

      
        Task<(bool Success, string Message, CurrentCapacityDto? Data)> GetCurrentCapacityAsync(int establishmentId);

  
        Task<(bool Success, string Message, IEnumerable<UserAccessHistoryDto>? Data)> GetAccessHistoryAsync(
            int establishmentId, DateTime startDate, DateTime endDate);

        Task<(bool Success, string Message, IEnumerable<HourlyAverageDto>? Data)> GetHourlyAveragesAsync(
            int establishmentId, DateTime startDate, DateTime endDate);
    }
}
