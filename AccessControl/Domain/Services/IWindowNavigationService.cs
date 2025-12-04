using System.Windows;

namespace AccessControl.Domain.Services
{
    public interface IWindowNavigationService
    {
        void Show<TWindow>() where TWindow : Window;
        bool? ShowDialog<TWindow>() where TWindow : Window;

        void OpenAsCurrent<TCurrentWindow, TNextWindow>(Action<TNextWindow> preconfig) where TCurrentWindow : Window where TNextWindow : Window;
        void OpenAsCurrent<TCurrentWindow, TNextWindow, TViewModel>(Action<TViewModel> configureViewModel)
            where TCurrentWindow : Window
            where TNextWindow : Window;
        void OpenAsCurrent<TCurrentWindow, TNextWindow>() where TCurrentWindow : Window where TNextWindow : Window;

        void Close<TWindow>() where TWindow : Window;

    }
}
