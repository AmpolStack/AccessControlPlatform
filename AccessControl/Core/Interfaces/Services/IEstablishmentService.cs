namespace AccessControl.Core.Interfaces.Services
{
    public interface IEstablishmentService
    {
        Task<(bool Success, string Message)> OpenEstablishmentAsync(int establishmentId, int userId);

        Task<(bool Success, string Message)> CloseEstablishmentAsync(int establishmentId, int userId);
    }
}
