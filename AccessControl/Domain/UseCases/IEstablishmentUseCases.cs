using AccessControl.Domain.Models;

namespace AccessControl.Domain.UseCases
{
    public interface IEstablishmentUseCase
    {
        Task<(bool Success, string Message)> OpenEstablishmentAsync(int establishmentId, int userId);
        Task<(bool Success, string Message, Establishment? Establishment)> GetEstablishmentAsync(int establishmentId);
        Task<(bool Success, string Message, int EstablishmentId)> CreateEstablishmentAsync(Establishment establishment, User? user = null);
        Task<(bool Success, string Message, int EstablishmentId)> UpdateEstablishmentAsync(Establishment establishment);
        Task<(bool Success, string Message)> CloseEstablishmentAsync(int establishmentId, int userId);
    }
}
