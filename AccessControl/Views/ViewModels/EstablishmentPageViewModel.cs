using AccessControl.Domain.Models;
using AccessControl.Domain.UseCases;
using AccessControl.Infraestructure.Common;
using System.Windows.Input;

namespace AccessControl.Views.ViewModels
{
    public class EstablishmentPageViewModel : ObservableObject
    {
        private readonly IEstablishmentUseCase _establishmentUseCase;

        private Establishment _establishment;
        private string _name = string.Empty;
        private string _description = string.Empty;
        private string _city = string.Empty;
        private string _address = string.Empty;
        private string _phoneNumber = string.Empty;
        private string _email = string.Empty;
        private int _maxCapacity;
        private bool _isOpen;
        private bool _isLoading;

        // Backup fields for Cancel operation
        private string _originalName = string.Empty;
        private string _originalDescription = string.Empty;
        private string _originalCity = string.Empty;
        private string _originalAddress = string.Empty;
        private string _originalPhoneNumber = string.Empty;
        private string _originalEmail = string.Empty;
        private int _originalMaxCapacity;

        private bool _isEditing;
        private int _establishmentId;
        private int _userId;

        public ICommand EditCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand DeleteEstablishmentCommand { get; }

        public ICommand InitCommand { get; }
        public AsyncRelayCommand OpenEstablishmentCommand { get; }
        public AsyncRelayCommand CloseEstablishmentCommand { get; }

        public EstablishmentPageViewModel(IEstablishmentUseCase establishmentUseCase)
        {
            _establishmentUseCase = establishmentUseCase;

            InitCommand = new AsyncRelayCommand(async _ => await LoadInfo());

            EditCommand = new RelayCommand(_ => StartEditing());
            SaveCommand = new AsyncRelayCommand(async _ => await SaveChanges());
            CancelCommand = new RelayCommand(_ => CancelEditing());
            DeleteEstablishmentCommand = new RelayCommand(_ => DeleteEstablishment());

            OpenEstablishmentCommand = new AsyncRelayCommand(
                execute: async _ => await OpenEstablishmentAsync()
            );

            CloseEstablishmentCommand = new AsyncRelayCommand(
                execute: async _ => await CloseEstablishmentAsync()
            );
        }

        #region Properties

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string City
        {
            get => _city;
            set => SetProperty(ref _city, value);
        }

        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public int MaxCapacity
        {
            get => _maxCapacity;
            set => SetProperty(ref _maxCapacity, value);
        }

        public bool IsOpen
        {
            get => _isOpen;
            set => SetProperty(ref _isOpen, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                if (SetProperty(ref _isEditing, value))
                {
                    OnPropertyChanged(nameof(IsReadOnly));
                }
            }
        }

        public bool IsReadOnly => !IsEditing;

        #endregion

        #region Methods

        public void Initialize(int establishmentId)
        {
            _establishmentId = establishmentId;
        }

        public async Task LoadInfo()
        {
            var resp = await _establishmentUseCase.GetEstablishmentAsync(_establishmentId);

            if (resp.Success && resp.Establishment != null)
            {
                _establishment = resp.Establishment;
                Name = _establishment.Name;
                Description = _establishment.Description ?? "";
                City = _establishment.City ?? "";
                Address = _establishment.Address ?? "";
                PhoneNumber = _establishment.PhoneNumber ?? "";
                Email = _establishment.Email ?? "";
                MaxCapacity = _establishment.MaxCapacity ?? 0;
            }

        }


        private async Task OpenEstablishmentAsync()
        {
            try
            {
                IsLoading = true;
                var (success, message) = await _establishmentUseCase.OpenEstablishmentAsync(_establishmentId, _userId);

                if (success)
                {
                    IsOpen = true;
                    System.Diagnostics.Debug.WriteLine("Establecimiento abierto exitosamente");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Error al abrir establecimiento: {message}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error crítico al abrir establecimiento: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task CloseEstablishmentAsync()
        {
            try
            {
                IsLoading = true;
                var (success, message) = await _establishmentUseCase.CloseEstablishmentAsync(_establishmentId, _userId);

                if (success)
                {
                    IsOpen = false;
                    System.Diagnostics.Debug.WriteLine("Establecimiento cerrado exitosamente");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Error al cerrar establecimiento: {message}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error crítico al cerrar establecimiento: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void StartEditing()
        {
            // Backup current data
            _originalName = Name;
            _originalDescription = Description;
            _originalCity = City;
            _originalAddress = Address;
            _originalPhoneNumber = PhoneNumber;
            _originalEmail = Email;
            _originalMaxCapacity = MaxCapacity;

            IsEditing = true;
        }

        private async Task SaveChanges()
        {
            _establishment.Name = Name;
            _establishment.Description = Description;
            _establishment.City = City;
            _establishment.Address = Address;
            _establishment.PhoneNumber = PhoneNumber;
            _establishment.Email = Email;
            _establishment.MaxCapacity = MaxCapacity;

            var resp = await _establishmentUseCase.UpdateEstablishmentAsync(_establishment);

            IsEditing = false;
        }

        private void CancelEditing()
        {
            Name = _originalName;
            Description = _originalDescription;
            City = _originalCity;
            Address = _originalAddress;
            PhoneNumber = _originalPhoneNumber;
            Email = _originalEmail;
            MaxCapacity = _originalMaxCapacity;

            IsEditing = false;
        }

        private void DeleteEstablishment()
        {

        }

        #endregion
    }
}
