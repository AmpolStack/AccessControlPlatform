using AccessControl.Core.Interfaces.Services;
using AccessControl.Infraestructure.Common;

namespace AccessControl.Core.ViewModels
{
    public class LoginViewModel : ObservableObject
    {
        private string _modalType = "Danger";
        private string _modalText = string.Empty;
        private bool _isModalVisible;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private readonly IUserService _userService;
        public RelayCommand LoginCommand { get; }

        public LoginViewModel(IUserService userService)
        {
            _userService = userService;

            LoginCommand = new RelayCommand(
                execute: _ => Login(),
                canExecute: _ => CanLogin()
            );
        }

        public string ModalType
        {
            get => _modalType;
            set => SetProperty(ref _modalType, value);
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

        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(Email)
                && !string.IsNullOrWhiteSpace(Password);
        }

        public string ModalText
        {
            get => _modalText;
            set => SetProperty(ref _modalText, value);
        }

        private async void Login()
        {
            try
            {
                var (Success, Message, User) = await _userService.LoginAsync(Email, Password);

                if (!Success)
                {
                    ModalType = "Danger";
                    ModalText = Message;
                    IsModalVisible = true;
                    return;
                }

                ModalType = "Success";
                ModalText = $"Bienvenido, {User!.FullName}";
                IsModalVisible = true;

                // Aquí navegarías al home
            }
            catch (Exception ex)
            {
                ModalType = "Danger";
                ModalText = $"Error inesperado: {ex.Message}";
                IsModalVisible = true;
            }
        }
    }
}
