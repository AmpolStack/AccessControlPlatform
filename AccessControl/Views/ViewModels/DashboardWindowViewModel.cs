using AccessControl.Domain.Models;
using AccessControl.Domain.Services;
using AccessControl.Domain.UseCases;
using AccessControl.Infraestructure.Common;
using AccessControl.Infraestructure.Dto;
using AccessControl.Views.Pages;
using AccessControl.Views.Windows;

namespace AccessControl.Views.ViewModels
{
    public class DashboardWindowViewModel : ObservableObject
    {
        private readonly IAccessRecordUseCase _accessRecordUseCase;
        private readonly IWindowNavigationService _windowNavigationService;
        private readonly IPageNavigationService _pageNavigationService;

        private UserLoginDataDto? _user;
        private string _userName = string.Empty;
        private string _establishmentName = string.Empty;
        private int _establishmentId;
        private int _userId;

        private int _currentCapacity;
        private int _maxCapacity;
        private string _capacityText = "AFORO: 0/0";
        private bool _isLoading;

        public AsyncRelayCommand LoadHomePageCommand { get; }
        public AsyncRelayCommand LoadProfilePageCommand { get; }
        public AsyncRelayCommand LoadEstablishmentPageCommand { get; }
        public AsyncRelayCommand LoadUsersPageCommand { get; }
        public AsyncRelayCommand LoadReportsPageCommand { get; }

        public AsyncRelayCommand LogOutCommand { get; }
        public RelayCommand InitializeNavigationCommand { get; }



        public DashboardWindowViewModel(
            IAccessRecordUseCase accessRecordUseCase,
            IWindowNavigationService windowNavigationService,
            IPageNavigationService pageNavigationService)
        {
            _accessRecordUseCase = accessRecordUseCase;
            _windowNavigationService = windowNavigationService;
            _pageNavigationService = pageNavigationService;

            LoadHomePageCommand = new AsyncRelayCommand(
                execute: async _ => await LoadHomePageAsync(),
                canExecute: _ => !IsLoading
            );

            LoadProfilePageCommand = new AsyncRelayCommand(
                execute: async _ => await LoadProfilePageAsync(),
                canExecute: _ => !IsLoading
            );

            LoadEstablishmentPageCommand = new AsyncRelayCommand(
                execute: async _ => await LoadEstablishmentPageAsync(),
                canExecute: _ => !IsLoading
            );

            LoadUsersPageCommand = new AsyncRelayCommand(
                execute: async _ => await LoadUsersPageAsync(),
                canExecute: _ => !IsLoading
            );

            LoadReportsPageCommand = new AsyncRelayCommand(
                execute: async _ => await LoadReportsPageAsync(),
                canExecute: _ => !IsLoading
            );

            LogOutCommand = new AsyncRelayCommand(execute: async _ => await LogOut());

            InitializeNavigationCommand = new RelayCommand(param =>
            {
                if (param is System.Windows.Controls.Frame frame)
                {
                    _pageNavigationService.Initialize(frame);
                }
            });
        }

        #region Properties

        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        public string EstablishmentName
        {
            get => _establishmentName;
            set => SetProperty(ref _establishmentName, value);
        }

        public int EstablishmentId
        {
            get => _establishmentId;
            set => SetProperty(ref _establishmentId, value);
        }

        public string CapacityText
        {
            get => _capacityText;
            set => SetProperty(ref _capacityText, value);
        }

        public int CurrentCapacity
        {
            get => _currentCapacity;
            set
            {
                if (SetProperty(ref _currentCapacity, value))
                    UpdateCapacityText();
            }
        }

        public int MaxCapacity
        {
            get => _maxCapacity;
            set
            {
                if (SetProperty(ref _maxCapacity, value))
                    UpdateCapacityText();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (SetProperty(ref _isLoading, value))
                {
                    LoadHomePageCommand.RaiseCanExecuteChanged();
                    LoadProfilePageCommand.RaiseCanExecuteChanged();
                    LoadEstablishmentPageCommand.RaiseCanExecuteChanged();
                    LoadUsersPageCommand.RaiseCanExecuteChanged();
                    LoadReportsPageCommand.RaiseCanExecuteChanged();
                }
            }
        }

        #endregion

        #region Private Methods

        private void UpdateCapacityText()
        {
            if (MaxCapacity > 0)
            {
                CapacityText = $"AFORO: {CurrentCapacity}/{MaxCapacity}";
            }
            else
            {
                CapacityText = $"AFORO: {CurrentCapacity}";
            }
        }

        #endregion

        #region Command Methods

        private async Task LoadHomePageAsync()
        {
            try
            {
                IsLoading = true;

                // Load capacity
                var (capacitySuccess, capacityMessage, capacityData) =
                    await _accessRecordUseCase.GetCurrentCapacityAsync(EstablishmentId);

                if (capacitySuccess && capacityData != null)
                {
                    CurrentCapacity = capacityData.CurrentCapacity;
                    MaxCapacity = capacityData.MaxCapacity ?? 0;
                }

                // Navigate to HomePage configuring the injected ViewModel
                // ViewLoadedBehavior will trigger LoadDataCommand automatically
                _pageNavigationService.Navigate<HomePage, HomePageViewModel>(vm =>
                {
                    vm.Initialize(UserName, EstablishmentId);
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading home page: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadProfilePageAsync()
        {
            try
            {
                IsLoading = true;

                // Navigate to ProfilePage configuring the injected ViewModel
                _pageNavigationService.Navigate<ProfilePage, ProfilePageViewModel>(vm =>
                {
                    vm.Initialize(_user?.FullName ?? "", 
                        _user?.Email ?? "",
                        _user?.IdentityDocument ?? "",
                        _user?.PhoneNumber ?? "");
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading profile page: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadEstablishmentPageAsync()
        {
            try
            {
                IsLoading = true;

                // Navigate to EstablishmentPage configuring the injected ViewModel

                _pageNavigationService.Navigate<EstablishmentPage, EstablishmentPageViewModel>(vm =>
                {
                    vm.Initialize(_establishmentId);
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading establishment page: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadUsersPageAsync()
        {
            try
            {
                IsLoading = true;

                // Navigate to UsersPage configuring the injected ViewModel
                _pageNavigationService.Navigate<UsersPage, UsersPageViewModel>(vm =>
                {
                    vm.Initialize(EstablishmentId);
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading users page: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadReportsPageAsync()
        {
            try
            {
                IsLoading = true;

                // Navigate to ReportsPage configuring the injected ViewModel
                _pageNavigationService.Navigate<ReportsPage, ReportsPageViewModel>(vm =>
                {
                    vm.Initialize(EstablishmentId, EstablishmentName);
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading reports page: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LogOut()
        {
            var logoutResponse = await _accessRecordUseCase.RegisterExitAsync(_userId, EstablishmentId);
            _windowNavigationService.OpenAsCurrent<DashboardWindow, MainWindow>();
        }

        #endregion

        #region Public Methods

        public void Initialize(UserLoginDataDto user)
        {
            _user = user;
            UserName = user.FullName;
            EstablishmentId = user.EstablishmentId;
            EstablishmentName = user.EstablishmentName;
            MaxCapacity = user.MaxCapacity ?? 0;
            _userId = user.Id;
        }

        #endregion
    }

    public class AccessRecordDisplayDto
    {
        public string Fecha { get; set; } = string.Empty;
        public string Hora { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string DocumentoIdentidad { get; set; } = string.Empty;
        public string HoraSalida { get; set; } = string.Empty;
        public int DuracionMinutos { get; set; }
    }
}
