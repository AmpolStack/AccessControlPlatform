using AccessControl.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace AccessControl.Infraestructure.Services
{
    public class PageNavigationService : IPageNavigationService
    {
        private readonly IServiceProvider _provider;
        private Frame? _frame;

        public PageNavigationService(IServiceProvider provider)
        {
            _provider = provider;
        }

        public void Initialize(Frame frame)
        {
            _frame = frame;
        }

        public void Navigate<TPage>() where TPage : Page
        {
            if (_frame == null) return;

            var page = _provider.GetRequiredService<TPage>();
            _frame.Navigate(page);
        }

        public void Navigate<TPage>(object parameter) where TPage : Page
        {
            if (_frame == null) return;

            var page = _provider.GetRequiredService<TPage>();

            // If parameter is provided, set it as DataContext before navigation
            if (parameter != null)
            {
                page.DataContext = parameter;
            }

            _frame.Navigate(page);
        }

        public void Navigate<TPage, TViewModel>(Action<TViewModel> configure) where TPage : Page where TViewModel : class
        {
            if (_frame == null) return;

            var page = _provider.GetRequiredService<TPage>();

            if (page.DataContext is TViewModel viewModel)
            {
                configure?.Invoke(viewModel);
            }

            _frame.Navigate(page);
        }

        public bool CanGoBack => _frame?.CanGoBack ?? false;

        public void GoBack()
        {
            if (_frame == null) return;

            if (CanGoBack)
                _frame.GoBack();
        }
    }
}
