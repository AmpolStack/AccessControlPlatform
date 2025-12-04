using AccessControl.Infraestructure.Dto;

namespace AccessControl.Domain.UseCases
{
    public interface IAccessRecordUseCase
    {
        Task<(bool Success, string Message)> RegisterEntryAsync(int userId, int establishmentId);

        Task<(bool Success, string Message)> RegisterExitAsync(int userId, int establishmentId);


        Task<(bool Success, string Message, CurrentCapacityDto? Data)> GetCurrentCapacityAsync(int establishmentId);


        Task<(bool Success, string Message, IEnumerable<UserAccessHistoryDto>? Data)> GetAccessHistoryAsync(
            int establishmentId, DateTime startDate, DateTime endDate);

        Task<(bool Success, string Message, IEnumerable<HourlyAverageDto>? Data)> GetHourlyAveragesAsync(
            int establishmentId, DateTime startDate, DateTime endDate);
    }
}
