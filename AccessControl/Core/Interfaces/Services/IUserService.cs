using AccessControl.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<(bool Success, string Message, User? User)> LoginAsync(string email, string password);
        Task<(bool Success, string Message)> CreateUserAsync(User user);
        Task<(bool Success, string Message)> UpdateUserAsync(User user);
        Task<(bool Success, string Message)> DeleteUserAsync(int userId);
        Task<IEnumerable<User>> GetAllUsersAsync();
    }
}
