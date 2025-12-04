using AccessControl.Domain.Models;
using AccessControl.Domain.Services;

namespace AccessControl.Infraestructure.Common
{
    public class SessionContext : ObservableObject, ISessionContext
    {
        private User? _currentUser;

        public User? CurrentUser
        {
            get => _currentUser;
            private set => SetProperty(ref _currentUser, value);
        }

        public void SetCurrentUser(User user)
        {
            CurrentUser = user;
        }

        public void ClearSession()
        {
            CurrentUser = null;
        }
    }
}
