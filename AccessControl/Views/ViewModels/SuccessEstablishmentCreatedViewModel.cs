using AccessControl.Domain.Services;
using AccessControl.Infraestructure.Common;
using AccessControl.Views.Windows;

namespace AccessControl.Views.ViewModels
{
    public class SuccessEstablishmentCreatedViewModel : ObservableObject
    {
        private readonly IWindowNavigationService _windowNavigationService;
        private string _establishmentName = string.Empty;

        public RelayCommand BackToLoginCommand { get; }

        public SuccessEstablishmentCreatedViewModel(IWindowNavigationService windowNavigationService)
        {
            _windowNavigationService = windowNavigationService;

            BackToLoginCommand = new RelayCommand(_ => BackToLogin());
        }

        public void BackToLogin()
        {
            _windowNavigationService.OpenAsCurrent<SuccessfulEstablishmentCreation, MainWindow>();
        }

        public string EstablishmentName
        {
            get => _establishmentName;
            set => SetProperty(ref _establishmentName, value);
        }

        public void Initialize(string establishmentName)
        {
            EstablishmentName = establishmentName;
        }


    }
}
