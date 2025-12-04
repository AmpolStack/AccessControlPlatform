using AccessControl.Views.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace AccessControl.Infraestructure.Common
{
    public class ViewModelLocator
    {
        public static LoginViewModel LoginViewModel => App.Services!.GetService<LoginViewModel>()!;
        public static SuccessEstablishmentCreatedViewModel SuccessEstablishmentCreatedViewModel => App.Services!.GetService<SuccessEstablishmentCreatedViewModel>()!;
        public static EstablishmentCreationViewModel EstablishmentCreationViewModel => App.Services!.GetService<EstablishmentCreationViewModel>()!;

        public static DashboardWindowViewModel DashboardWindowViewModel => App.Services!.GetService<DashboardWindowViewModel>()!;
        public static CreateUserViewModel CreateUserViewModel => App.Services!.GetService<CreateUserViewModel>()!;
    }
}
