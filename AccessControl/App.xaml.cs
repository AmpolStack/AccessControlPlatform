using AccessControl.Domain.Models;
using AccessControl.Domain.Services;
using AccessControl.Domain.UseCases;
using AccessControl.Infraestructure.Services;
using AccessControl.Infraestructure.UseCases;
using AccessControl.Views.Pages;
using AccessControl.Views.ViewModels;
using AccessControl.Views.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace AccessControl
{
    public partial class App : Application
    {
        public static IServiceProvider? Services { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            // Windows 
            services.AddTransient<MainWindow>();
            services.AddTransient<EstablishmentCreatonWindow>();
            services.AddTransient<ExtraOrdinaryWindow>();
            services.AddTransient<DashboardWindow>();
            services.AddTransient<SuccessfulEstablishmentCreation>();
            services.AddTransient<CreateUserWindow>();

            // Pages
            services.AddTransient<HomePage>();
            services.AddTransient<ProfilePage>();
            services.AddTransient<EstablishmentPage>();
            services.AddTransient<UsersPage>();
            services.AddTransient<ReportsPage>();

            // Common Services
            services.AddScoped<IUnitOrWork, UnitOfWork>();
            services.AddScoped<ITransactionParameterHelpers, TransactionParameterHelpers>();
            services.AddSingleton<IWindowNavigationService, WindowNavigationService>();
            services.AddSingleton<IPageNavigationService, PageNavigationService>();
            services.AddScoped<IHashingService, HashingService>();

            // Business Services
            services.AddScoped<IUserUseCases, UserUseCases>();
            services.AddScoped<IAccessRecordUseCase, AccessRecordUseCase>();
            services.AddScoped<IEstablishmentUseCase,EstablishmentUseCase>();

            // ViewModels
            services.AddScoped<LoginViewModel>();
            services.AddScoped<EstablishmentCreationViewModel>();
            services.AddTransient<DashboardWindowViewModel>(); // Transient para nueva instancia
            services.AddTransient<SuccessEstablishmentCreatedViewModel>();
            services.AddTransient<HomePageViewModel>();
            services.AddTransient<ProfilePageViewModel>();
            services.AddTransient<EstablishmentPageViewModel>();
            services.AddTransient<UsersPageViewModel>();
            services.AddTransient<ReportsPageViewModel>();
            services.AddScoped<CreateUserViewModel>();

            // DbContext
            services.AddDbContext<AccessControlContext>(opt =>
            {
                opt.UseSqlServer("Server=Me1097;Database=AccessControl;Trusted_Connection=true;TrustServerCertificate=true;");
            });


            Services = services.BuildServiceProvider();

            base.OnStartup(e);

            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }

}
