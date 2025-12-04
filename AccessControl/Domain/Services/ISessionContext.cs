using AccessControl.Domain.Models;

namespace AccessControl.Domain.Services
{
    public interface ISessionContext
    {
        User? CurrentUser { get; }
        void SetCurrentUser(User user);
        void ClearSession();

    }
}
