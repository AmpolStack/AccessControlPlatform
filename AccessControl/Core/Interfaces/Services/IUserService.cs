using AccessControl.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Infraestructure.Services
{
    public interface IUserService
    {
        Task<(bool Success, string Message, User? User)> LoginAsync(string email, string password);
        Task<(bool Success, string Message)> RegisterAsync(User user);
        Task<(bool Success, string Message)> UpdateAsync(User user);
        Task<(bool Success, string Message)> DeleteAsync(int userId);
        Task<List<User>> GetAllAsync();
    }
}
