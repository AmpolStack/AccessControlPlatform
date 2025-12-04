using AccessControl.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace AccessControl.Infraestructure.Services
{
    public class WindowNavigationService : IWindowNavigationService
    {
        private readonly IServiceProvider _provider;

        public WindowNavigationService(IServiceProvider provider)
        {
            _provider = provider;
        }

        public void Close<TWindow>() where TWindow : Window
        {
            var currentWindow = Application.Current.Windows
              .OfType<TWindow>()
              .FirstOrDefault();

            currentWindow?.Close();
        }

        public void OpenAsCurrent<TCurrentWindow, TNextWindow>(Action<TNextWindow> preconfig)
            where TCurrentWindow : Window
            where TNextWindow : Window
        {
            var newWindow = _provider.GetRequiredService<TNextWindow>();

            if (newWindow == null)
                return;

            preconfig?.Invoke(newWindow);

            var currentWindow = Application.Current.Windows
               .OfType<TCurrentWindow>()
               .FirstOrDefault();

            newWindow.Show();

            Application.Current.MainWindow = newWindow;

            currentWindow?.Close();
        }

        public void OpenAsCurrent<TCurrentWindow, TNextWindow, TViewModel>(Action<TViewModel> configureViewModel)
            where TCurrentWindow : Window
            where TNextWindow : Window
        {
            var newWindow = _provider.GetRequiredService<TNextWindow>();

            if (newWindow == null)
                return;

            // Configurar el ViewModel si el DataContext coincide con el tipo esperado
            if (newWindow.DataContext is TViewModel viewModel)
            {
                configureViewModel?.Invoke(viewModel);
            }

            var currentWindow = Application.Current.Windows
               .OfType<TCurrentWindow>()
               .FirstOrDefault();

            newWindow.Show();

            Application.Current.MainWindow = newWindow;

            currentWindow?.Close();
        }

        public void OpenAsCurrent<TCurrentWindow, TNextWindow>()
            where TCurrentWindow : Window
            where TNextWindow : Window
        {
            var newWindow = _provider.GetRequiredService<TNextWindow>();

            if (newWindow == null)
                return;

            // Buscar la ventana actual
            var currentWindow = Application.Current.Windows
                .OfType<TCurrentWindow>()
                .FirstOrDefault();

            newWindow.Show();

            Application.Current.MainWindow = newWindow;

            currentWindow?.Close();
        }

        public void Show<TWindow>() where TWindow : Window
        {
            var window = _provider.GetRequiredService<TWindow>();

            if (window != null && !window.IsLoaded || window?.IsVisible == false)
            {
                window?.Show();
            }
        }

        public bool? ShowDialog<TWindow>() where TWindow : Window
        {
            var window = _provider.GetRequiredService<TWindow>();
            return window?.ShowDialog();
        }
    }
}
