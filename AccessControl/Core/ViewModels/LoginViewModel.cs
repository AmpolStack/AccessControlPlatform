using AccessControl.Core.Interfaces.Services;
using AccessControl.Infraestructure.Common;

namespace AccessControl.Core.ViewModels
{
    public class LoginViewModel : ObservableObject
    {
        private readonly IUserService _userService;
        
        private string _modalType = "Danger";
        private string _modalText = string.Empty;
        private bool _isModalVisible;
        private string _email = "empleado1@gimnasio.com";
        private string _password = "$2a$10$1234512345123451234512";
        private string _name;
        private bool _isLoading;

        public AsyncRelayCommand LoginCommand { get; }

        public LoginViewModel(IUserService userService)
        {
            _userService = userService;

            LoginCommand = new AsyncRelayCommand(
                execute: async _ => await LoginAsync(),
                canExecute: _ => CanLogin()
            );
        }

        public string Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value))
                    LoginCommand.RaiseCanExecuteChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (SetProperty(ref _name, value))
                    LoginCommand.RaiseCanExecuteChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value))
                    LoginCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsModalVisible
        {
            get => _isModalVisible;
            set => SetProperty(ref _isModalVisible, value);
        }

        public string ModalType
        {
            get => _modalType;
            set => SetProperty(ref _modalType, value);
        }

        public string ModalText
        {
            get => _modalText;
            set => SetProperty(ref _modalText, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(Email) 
                && !string.IsNullOrWhiteSpace(Password)
                && !IsLoading;
        }

        private async Task LoginAsync()
        {
            try
            {
                IsLoading = true;
                LoginCommand.RaiseCanExecuteChanged();

                var (Success, Message, User) = await _userService.LoginAsync(Email, Password);

                if (!Success || User==null)
                {
                    ShowModal("Danger", Message);
                    return;
                }

                Name = User.FullName;
                ShowModal("Success", Message);
                
                // TODO: Implementar navegación
                // NavigationService.NavigateTo(typeof(HomeViewModel));
            }
            catch (Exception ex)
            {
                ShowModal("Danger", $"Error inesperado: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
                LoginCommand.RaiseCanExecuteChanged();
            }
        }

        private void ShowModal(string type, string text)
        {
            ModalType = type;
            ModalText = text;
            IsModalVisible = true;
        }
    }
}
