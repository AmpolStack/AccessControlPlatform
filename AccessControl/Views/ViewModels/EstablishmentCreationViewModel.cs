using AccessControl.Domain.Models;
using AccessControl.Domain.Services;
using AccessControl.Domain.UseCases;
using AccessControl.Infraestructure.Common;
using AccessControl.Views.Windows;

namespace AccessControl.Views.ViewModels
{
    public class EstablishmentCreationViewModel : ObservableObject
    {
        private readonly IWindowNavigationService _navigationService;
        private readonly IEstablishmentUseCase _establishmentUseCases;

        // Establishment properties
        private string _name = string.Empty;
        private string _description = string.Empty;
        private string _city = string.Empty;
        private string _address = string.Empty;
        private string _email = string.Empty;
        private string _phoneNumber = string.Empty;

        // User properties
        private string _userFullName = string.Empty;
        private string _userEmail = string.Empty;
        private string _userPassword = string.Empty;
        private string _identityDocument = string.Empty;
        private string _userPasswordConfirmation = string.Empty;

        // Modal properties
        private string _modalType = "Danger";
        private string _modalText = string.Empty;
        private bool _isModalVisible;

        // UI State
        private bool _isLoading;

        // Commands
        public RelayCommand BackToLoginCommand { get; }
        public AsyncRelayCommand CreateEstablishmentCommand { get; }

        public EstablishmentCreationViewModel(
            IWindowNavigationService navigationService,
            IEstablishmentUseCase establishmentUseCases)
        {
            _navigationService = navigationService;
            _establishmentUseCases = establishmentUseCases;

            BackToLoginCommand = new RelayCommand(_ => BackToLogin());

            CreateEstablishmentCommand = new AsyncRelayCommand(
                execute: async _ => await CreateEstablishmentAsync(),
                canExecute: _ => CanCreateEstablishment()
            );
        }

        #region Properties

        // Establishment Properties
        public string Name
        {
            get => _name;
            set
            {
                if (SetProperty(ref _name, value))
                    CreateEstablishmentCommand.RaiseCanExecuteChanged();
            }
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string City
        {
            get => _city;
            set
            {
                if (SetProperty(ref _city, value))
                    CreateEstablishmentCommand.RaiseCanExecuteChanged();
            }
        }

        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }

        // User Properties
        public string UserFullName
        {
            get => _userFullName;
            set
            {
                if (SetProperty(ref _userFullName, value))
                    CreateEstablishmentCommand.RaiseCanExecuteChanged();
            }
        }

        public string IdentityDocument
        {
            get => _identityDocument;
            set
            {
                if (SetProperty(ref _identityDocument, value))
                    CreateEstablishmentCommand.RaiseCanExecuteChanged();
            }
        }
        public string UserEmail
        {
            get => _userEmail;
            set
            {
                if (SetProperty(ref _userEmail, value))
                    CreateEstablishmentCommand.RaiseCanExecuteChanged();
            }
        }

        public string UserPassword
        {
            get => _userPassword;
            set
            {
                if (SetProperty(ref _userPassword, value))
                    CreateEstablishmentCommand.RaiseCanExecuteChanged();
            }
        }

        public string UserPasswordConfirmation
        {
            get => _userPasswordConfirmation;
            set
            {
                if (SetProperty(ref _userPasswordConfirmation, value))
                    CreateEstablishmentCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (SetProperty(ref _isLoading, value))
                {
                    BackToLoginCommand.RaiseCanExecuteChanged();
                    CreateEstablishmentCommand.RaiseCanExecuteChanged();
                }
            }
        }

        // Modal Properties
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

        #endregion

        #region Methods

        private bool CanCreateEstablishment()
        {
            return !string.IsNullOrWhiteSpace(Name) &&
                   !string.IsNullOrWhiteSpace(City) &&
                   !string.IsNullOrWhiteSpace(UserFullName) &&
                   !string.IsNullOrWhiteSpace(IdentityDocument) &&  // ✅ Validar documento
                   !string.IsNullOrWhiteSpace(UserEmail) &&
                   !string.IsNullOrWhiteSpace(UserPassword) &&
                   !string.IsNullOrWhiteSpace(UserPasswordConfirmation) &&
                   !IsLoading;
        }

        private async Task CreateEstablishmentAsync()
        {
            try
            {
                IsLoading = true;

                if (UserPassword != UserPasswordConfirmation)
                {
                    ShowModal("Danger", "Las contraseñas no coinciden");
                    return;
                }

                // Hash de la contraseña usando BCrypt
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(UserPassword);

                // Crear establecimiento
                var establishment = new Establishment
                {
                    Name = Name,
                    Description = string.IsNullOrWhiteSpace(Description) ? null : Description,
                    City = City,
                    Address = string.IsNullOrWhiteSpace(Address) ? null : Address,
                    Email = string.IsNullOrWhiteSpace(Email) ? null : Email,
                    PhoneNumber = string.IsNullOrWhiteSpace(PhoneNumber) ? null : PhoneNumber,
                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    MaxCapacity = null
                };

                var user = new User
                {
                    FullName = UserFullName,
                    IdentityDocument = IdentityDocument,
                    Password = hashedPassword,
                    Email = UserEmail,
                    Role = "Admin",
                    IsActive = true
                };

                var (success, message, establishmentId) = await _establishmentUseCases.CreateEstablishmentAsync(establishment, user);

                if (!success)
                {
                    ShowModal("Danger", $"Error al crear establecimiento: {message}");
                    return;
                }

                _navigationService.OpenAsCurrent<EstablishmentCreatonWindow, SuccessfulEstablishmentCreation, SuccessEstablishmentCreatedViewModel>(
                    vm => vm.Initialize(establishment.Name)
                );

            }
            catch (Exception ex)
            {
                ShowModal("Danger", $"Error inesperado: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void BackToLogin()
        {
            _navigationService.OpenAsCurrent<EstablishmentCreatonWindow, MainWindow>();
        }

        private void ShowModal(string type, string message)
        {
            ModalType = type;
            ModalText = message;
            IsModalVisible = true;
        }

        #endregion
    }
}
