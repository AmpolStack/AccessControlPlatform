using AccessControl.Domain.Models;
using AccessControl.Domain.UseCases;
using AccessControl.Infraestructure.Common;
using System.Windows.Input;

namespace AccessControl.Views.ViewModels
{
    public class ProfilePageViewModel : ObservableObject
    {
        private readonly IUserUseCases _userUseCases;

        private string _fullName = string.Empty;
        private string _email = string.Empty;
        private string _identityDocument = string.Empty;
        private string _phone = string.Empty;

        // Backup fields for Cancel operation
        private string _originalFullName = string.Empty;
        private string _originalEmail = string.Empty;
        private string _originalIdentityDocument = string.Empty;
        private string _originalPhone = string.Empty;

        private bool _isEditing;

        public ICommand EditCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand UpdatePasswordCommand { get; }

        public ProfilePageViewModel(IUserUseCases userUseCases)
        {
            _userUseCases = userUseCases;

            EditCommand = new RelayCommand(_ => StartEditing());
            SaveCommand = new AsyncRelayCommand(async _ => await SaveChanges());
            CancelCommand = new RelayCommand(_ => CancelEditing());
            UpdatePasswordCommand = new RelayCommand(_ => { /* TODO: Implement Password Update */ });
        }

        #region Properties

        public string FullName
        {
            get => _fullName;
            set => SetProperty(ref _fullName, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string IdentityDocument
        {
            get => _identityDocument;
            set => SetProperty(ref _identityDocument, value);
        }

        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
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

        public void Initialize(string fullname, string email, string identityDocument, string phoneNumber)
        {
            FullName = fullname;
            Email = email;
            IdentityDocument = identityDocument; 
            Phone = phoneNumber ?? "";
        }

        private void StartEditing()
        {
            _originalFullName = FullName;
            _originalEmail = Email;
            _originalIdentityDocument = IdentityDocument;
            _originalPhone = Phone;

            IsEditing = true;
        }

        private async Task SaveChanges()
        {
            var userSearched = await _userUseCases.UpdatePropertiesAsync(Email, Phone, IdentityDocument, FullName);
            IsEditing = false;
        }

        private void CancelEditing()
        {
            // Restore original data
            FullName = _originalFullName;
            Email = _originalEmail;
            IdentityDocument = _originalIdentityDocument;
            Phone = _originalPhone;

            IsEditing = false;
        }

        #endregion
    }
}
