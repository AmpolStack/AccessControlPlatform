using AccessControl.Domain.Services;
using AccessControl.Infraestructure.Common;
using AccessControl.Views.Windows;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AccessControl.Views.ViewModels
{
    public class UsersPageViewModel : ObservableObject
    {
        private readonly IWindowNavigationService _windowNavigationService;
        private ObservableCollection<UserDisplayDto> _users = [];
        private bool _isLoading;
        private int _establishmentId;

        public ICommand CreateUserCommand { get; }
        public ICommand RefreshCommand { get; }

        public UsersPageViewModel(IWindowNavigationService windowNavigationService)
        {
            _windowNavigationService = windowNavigationService;
            CreateUserCommand = new RelayCommand(_ => CreateUser());
            RefreshCommand = new AsyncRelayCommand(async _ => await LoadUsersAsync());
        }

        #region Properties

        public ObservableCollection<UserDisplayDto> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        #endregion

        #region Methods

        public void Initialize(int establishmentId)
        {
            _establishmentId = establishmentId;
        }

        public async Task LoadUsersAsync()
        {
            try
            {
                IsLoading = true;

                // TODO: Llamar al servicio para cargar usuarios del establecimiento
                // var users = await _userService.GetUsersByEstablishmentAsync(_establishmentId);

                // Datos de ejemplo (placeholder)
                var usersList = new List<UserDisplayDto>
                {
                    new() {
                        Id = 1,
                        FullName = "Andres Gutierrez",
                        IdentityDocument = "52281959",
                        Email = "andres@example.com",
                        LastAccessDate = new DateTime(2024, 12, 3)
                    },
                    new() {
                        Id = 2,
                        FullName = "Maria Rodriguez",
                        IdentityDocument = "10923456",
                        Email = "maria@example.com",
                        LastAccessDate = new DateTime(2024, 12, 2)
                    },
                    new() {
                        Id = 3,
                        FullName = "Carlos Mendez",
                        IdentityDocument = "78654321",
                        Email = "carlos@example.com",
                        LastAccessDate = new DateTime(2024, 12, 1)
                    }
                };

                // Configurar comandos para cada usuario
                foreach (var user in usersList)
                {
                    user.EditCommand = new RelayCommand(_ => EditUser(user.Id));
                    user.DeleteCommand = new RelayCommand(_ => DeleteUser(user.Id));
                    user.ViewRecordsCommand = new RelayCommand(_ => ViewUserRecords(user.Id));
                }

                Users = new ObservableCollection<UserDisplayDto>(usersList);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading users: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void CreateUser()
        {
            // Abrir ventana de creación de usuario
            var window = _windowNavigationService.ShowDialog<CreateUserWindow>();

            // Recargar usuarios después de cerrar la ventana
            if (window == true)
            {
                LoadUsersAsync().ConfigureAwait(false);
            }
        }

        public static void EditUser(int userId)
        {
            // TODO: Navegar a modal/página de edición de usuario
            System.Diagnostics.Debug.WriteLine($"Edit User {userId} clicked");
        }

        public static void DeleteUser(int userId)
        {
            // TODO: Mostrar confirmación y eliminar usuario
            System.Diagnostics.Debug.WriteLine($"Delete User {userId} clicked");
        }

        public static void ViewUserRecords(int userId)
        {
            // TODO: Navegar a página de historial de usuario
            System.Diagnostics.Debug.WriteLine($"View Records for User {userId} clicked");
        }

        #endregion
    }

    // DTO para mostrar usuarios en la tabla
    public class UserDisplayDto : ObservableObject
    {
        private int _id;
        private string _fullName = string.Empty;
        private string _identityDocument = string.Empty;
        private string _email = string.Empty;
        private DateTime _lastAccessDate;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string FullName
        {
            get => _fullName;
            set => SetProperty(ref _fullName, value);
        }

        public string IdentityDocument
        {
            get => _identityDocument;
            set => SetProperty(ref _identityDocument, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public DateTime LastAccessDate
        {
            get => _lastAccessDate;
            set => SetProperty(ref _lastAccessDate, value);
        }

        public string LastAccessDateFormatted => LastAccessDate.ToString("dd/MM/yyyy");

        // TODO: IMPLEMENTS COMMANDS
        public RelayCommand? EditCommand { get; set; }
        public RelayCommand? DeleteCommand { get; set; }
        public RelayCommand? ViewRecordsCommand { get; set; }
    }
}
