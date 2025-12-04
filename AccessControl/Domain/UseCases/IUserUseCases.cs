using AccessControl.Domain.Models;
using AccessControl.Infraestructure.Dto;

namespace AccessControl.Domain.UseCases
{
    public interface IUserUseCases
    {
        Task<(bool Success, string Message, UserLoginDataDto? User)> LoginAsync(string email, string password);
        Task<(bool Success, string Message)> CreateUserAsync(User user);
        Task<(bool Success, string Message)> UpdatePropertiesAsync(string email,
            string phone, string identityDocument, string fullname);
        Task<(bool Success, string Message)> DeleteUserAsync(int userId);
        Task<IEnumerable<User>> GetAllUsersAsync();
    }
}
