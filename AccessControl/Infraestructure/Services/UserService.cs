using AccessControl.Core.Interfaces.Services;
using AccessControl.Core.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Infraestructure.Services
{
    public class UserService : IUserService
    {
        private readonly AccessControlContext _context;

        public UserService(AccessControlContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message, User? User)> LoginAsync(
        string email,
        string password)
        {
            try
            {
                var emailParam = new SqlParameter("@Email", email);
                var passParam = new SqlParameter("@Password", password);

                var userList = await _context.Users
                    .FromSqlRaw("EXEC sp_LoginUser @Email, @Password", emailParam, passParam)
                    .ToListAsync();

                if (userList.Count == 0)
                    return (false, "Credenciales incorrectas", null);

                return (true, "Login exitoso", userList.First());
            }
            catch (SqlException ex)
            {
                return (false, $"Error SQL: {ex.Message}", null);
            }
            catch (Exception ex)
            {
                return (false, $"Error inesperado: {ex.Message}", null);
            }
        }

        // ============================================================
        // REGISTRAR USUARIO (via Stored Procedure)
        // ============================================================
        public async Task<(bool Success, string Message)> RegisterAsync(User user)
        {
            try
            {
                var p1 = new SqlParameter("@Name", user.FullName);
                var p2 = new SqlParameter("@Email", user.Email);
                var p3 = new SqlParameter("@PasswordHash", user.Password);
                var p4 = new SqlParameter("@Role", user.Role);

                var pMessage = new SqlParameter("@Message", SqlDbType.NVarChar, 150)
                {
                    Direction = ParameterDirection.Output
                };
                var pSuccess = new SqlParameter("@Success", SqlDbType.Bit)
                {
                    Direction = ParameterDirection.Output
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_RegisterUser @Name, @Email, @PasswordHash, @Role, @Message OUTPUT, @Success OUTPUT",
                    p1, p2, p3, p4, pMessage, pSuccess
                );

                return ((bool)pSuccess.Value, (string)pMessage.Value);
            }
            catch (SqlException ex)
            {
                return (false, $"Error SQL: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"Error inesperado: {ex.Message}");
            }
        }

        // ============================================================
        // ACTUALIZAR USUARIO (SP o EF, depende de lo que tengas)
        // ============================================================
        public async Task<(bool Success, string Message)> UpdateAsync(User user)
        {
            try
            {
                // SUPONIENDO que tienes el SP sp_UpdateUser
                var p1 = new SqlParameter("@Id", user.Id);
                var p2 = new SqlParameter("@Name", user.Name);
                var p3 = new SqlParameter("@Email", user.Email);
                var p4 = new SqlParameter("@Role", user.Role);

                var pMessage = new SqlParameter("@Message", SqlDbType.NVarChar, 150)
                {
                    Direction = ParameterDirection.Output
                };
                var pSuccess = new SqlParameter("@Success", SqlDbType.Bit)
                {
                    Direction = ParameterDirection.Output
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_UpdateUser @Id, @Name, @Email, @Role, @Message OUTPUT, @Success OUTPUT",
                    p1, p2, p3, p4, pMessage, pSuccess
                );

                return ((bool)pSuccess.Value, (string)pMessage.Value);
            }
            catch (SqlException ex)
            {
                return (false, $"Error SQL: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"Error inesperado: {ex.Message}");
            }
        }

        // ============================================================
        // ELIMINAR USUARIO (via SP)
        // ============================================================
        public async Task<(bool Success, string Message)> DeleteAsync(int userId)
        {
            try
            {
                var pId = new SqlParameter("@Id", userId);

                var pMessage = new SqlParameter("@Message", SqlDbType.NVarChar, 150)
                {
                    Direction = ParameterDirection.Output
                };
                var pSuccess = new SqlParameter("@Success", SqlDbType.Bit)
                {
                    Direction = ParameterDirection.Output
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_DeleteUser @Id, @Message OUTPUT, @Success OUTPUT",
                    pId, pMessage, pSuccess
                );

                return ((bool)pSuccess.Value, (string)pMessage.Value);
            }
            catch (SqlException ex)
            {
                return (false, $"Error SQL: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"Error inesperado: {ex.Message}");
            }
        }

        // ============================================================
        // OBTENER TODOS LOS USUARIOS (NO SP, EF es suficiente)
        // ============================================================
        public async Task<List<User>> GetAllAsync()
        {
            try
            {
                return await _context.Users.AsNoTracking().ToListAsync();
            }
            catch
            {
                return new List<User>();
            }
        }
    }
}
