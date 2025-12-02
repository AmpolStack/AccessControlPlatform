using AccessControl.Core.Interfaces.Common;
using AccessControl.Core.Interfaces.Services;
using AccessControl.Core.Models;
using AccessControl.Infraestructure.Common;
using AccessControl.Infraestructure.Services;
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

            services.AddSingleton<MainWindow>();

            services.AddScoped<IUnitOrWork, UnitOfWork>();
            services.AddScoped<ITransactionParameterHelpers, TransactionParameterHelpers>();

            services.AddScoped<IUserService, UserService>();

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
