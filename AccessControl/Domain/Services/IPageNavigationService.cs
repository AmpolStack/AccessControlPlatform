using System.Windows.Controls;

namespace AccessControl.Domain.Services
{
    public interface IPageNavigationService
    {
        void Navigate<TPage>() where TPage : Page;
        void Navigate<TPage>(object parameter) where TPage : Page;
        void Navigate<TPage, TViewModel>(Action<TViewModel> configure) where TPage : Page where TViewModel : class;
        bool CanGoBack { get; }
        void GoBack();
        void Initialize(Frame frame);
    }
}
