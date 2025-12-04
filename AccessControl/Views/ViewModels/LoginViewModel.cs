using AccessControl.Domain.Services;
using AccessControl.Domain.UseCases;
using AccessControl.Infraestructure.Common;
using AccessControl.Views.Windows;

namespace AccessControl.Views.ViewModels
{
    public class LoginViewModel : ObservableObject
    {
        private readonly IUserUseCases _userUseCases;
        private readonly IWindowNavigationService _windowNavigationService;
        private readonly IAccessRecordUseCase _accessRecordUseCase;

        private string _modalType = "Danger";
        private string _modalText = string.Empty;
        private bool _isModalVisible;
        private string _email = "camilo@gmail.com";
        private string _password = "$2a$11$BbMn5GzV0QFMSrz86qebS.L.mRs5Nmqu3h.1V.J910iF.YjWm76RW";
        private string? _name;
        private bool _isLoading;

        public AsyncRelayCommand LoginCommand { get; }
        public RelayCommand OpenCreateEstablishmentWindowCommand { get; }

        public LoginViewModel(IUserUseCases userUseCases, 
            IWindowNavigationService windowNavigationService,
            IAccessRecordUseCase accessRecordUseCase)
        {
            _userUseCases = userUseCases;
            _windowNavigationService = windowNavigationService;
            _accessRecordUseCase = accessRecordUseCase;

            LoginCommand = new AsyncRelayCommand(
                execute: async _ => await LoginAsync(),
                canExecute: _ => CanLogin()
            );

            OpenCreateEstablishmentWindowCommand = new RelayCommand(
                execute: _ => _windowNavigationService.OpenAsCurrent<MainWindow, EstablishmentCreatonWindow>()
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

        public string? Name
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

                var (Success, Message, User) = await _userUseCases.LoginAsync(Email, Password);

                if (!Success || User == null)
                {
                    ShowModal("Danger", Message);
                    return;
                }

                var entry = await _accessRecordUseCase.RegisterEntryAsync(User.Id, User.EstablishmentId);

                if (!entry.Success)
                {
                    ShowModal("Danger", entry.Message);
                    return;
                }

                Name = User.FullName;

                _windowNavigationService.OpenAsCurrent<MainWindow, DashboardWindow, DashboardWindowViewModel>(conf =>
                    conf.Initialize(User)
                 );



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
