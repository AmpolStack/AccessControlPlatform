using AccessControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
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
