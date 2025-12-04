using AccessControl.Domain.Models;
using AccessControl.Domain.Services;
using AccessControl.Domain.UseCases;
using AccessControl.Infraestructure.Common;
using AccessControl.Views.Enums;
using AccessControl.Views.Windows;

namespace AccessControl.Views.ViewModels
{
    public class CreateUserViewModel : ObservableObject
    {
        private readonly IUserUseCases _userUseCases;
        private readonly IHashingService _hashingService;
        private readonly IWindowNavigationService _windowNavigationService;

        private string _fullName = string.Empty;
        private string _identityDocument = string.Empty;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private bool _isLoading;
        private int _establishmentId;

        // Modal properties
        private AppElementType _modalType = AppElementType.Sober;
        private string _modalText = string.Empty;
        private bool _isModalVisible;

        public AsyncRelayCommand CreateUserCommand { get; }
        public RelayCommand CancelCommand { get; }

        public CreateUserViewModel(IUserUseCases userUseCases, 
            IHashingService hashingService,
            IWindowNavigationService windowNavigationService)
        {
            _userUseCases = userUseCases;
            _hashingService = hashingService;
            _windowNavigationService = windowNavigationService;

            CreateUserCommand = new AsyncRelayCommand(
                execute: async _ => await CreateUserAsync(),
                canExecute: _ => CanCreateUser()
            );

            CancelCommand = new RelayCommand(_ => CloseCreateUserWindow());
        }

        #region Properties

        public string FullName
        {
            get => _fullName;
            set
            {
                if (SetProperty(ref _fullName, value))
                    CreateUserCommand.RaiseCanExecuteChanged();
            }
        }

        public string IdentityDocument
        {
            get => _identityDocument;
            set
            {
                if (SetProperty(ref _identityDocument, value))
                    CreateUserCommand.RaiseCanExecuteChanged();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value))
                    CreateUserCommand.RaiseCanExecuteChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value))
                    CreateUserCommand.RaiseCanExecuteChanged();
            }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                if (SetProperty(ref _confirmPassword, value))
                    CreateUserCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public AppElementType ModalType
        {
            get => _modalType;
            set => SetProperty(ref _modalType, value);
        }

        public string ModalText
        {
            get => _modalText;
            set => SetProperty(ref _modalText, value);
        }

        public bool IsModalVisible
        {
            get => _isModalVisible;
            set => SetProperty(ref _isModalVisible, value);
        }

        #endregion

        #region Methods

        public void Initialize(int establishmentId)
        {
            _establishmentId = establishmentId;
        }

        private bool CanCreateUser()
        {
            return !string.IsNullOrWhiteSpace(FullName) &&
                   !string.IsNullOrWhiteSpace(IdentityDocument) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !string.IsNullOrWhiteSpace(ConfirmPassword) &&
                   !IsLoading;
        }

        private async Task CreateUserAsync()
        {
            try
            {
                IsLoading = true;

                // Validate Password
                if (Password != ConfirmPassword)
                {
                    ShowModal(AppElementType.Danger, "Las contraseñas no coinciden");
                    return;
                }

                var hashedPassword = _hashingService.GetHash(Password);

                var (success, message) = await _userUseCases.CreateUserAsync(new User 
                {
                    FullName = FullName,
                    IdentityDocument = IdentityDocument,
                    Email = Email,
                    Password = hashedPassword,
                    EstablishmentId = _establishmentId
                });


                if (success)
                {
                    ShowModal(AppElementType.Success, message);

                    // Close window after 2 seconds
                    await Task.Delay(2000);

                    // This will close the current window
                    CloseCreateUserWindow();
                }
                else
                {
                    ShowModal(AppElementType.Danger, message);
                }
            }
            catch (Exception ex)
            {
                ShowModal(AppElementType.Danger, $"Error al crear usuario: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
                CreateUserCommand.RaiseCanExecuteChanged();
            }
        }

        private void CloseCreateUserWindow()
        {
            _windowNavigationService.Close<CreateUserWindow>();
        }

        private void ShowModal(AppElementType type, string text)
        {
            ModalType = type;
            ModalText = text;
            IsModalVisible = true;
        }

        #endregion
    }
}
