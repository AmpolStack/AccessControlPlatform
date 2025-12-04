using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AccessControl.Infraestructure.Common
{
    public static class FrameBehavior
    {
        public static readonly DependencyProperty InitializationCommandProperty =
            DependencyProperty.RegisterAttached(
                "InitializationCommand",
                typeof(ICommand),
                typeof(FrameBehavior),
                new PropertyMetadata(null, OnInitializationCommandChanged));

        public static ICommand GetInitializationCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(InitializationCommandProperty);
        }

        public static void SetInitializationCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(InitializationCommandProperty, value);
        }

        private static void OnInitializationCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Frame frame && e.NewValue is ICommand command)
            {
                if (frame.IsLoaded)
                {
                    command.Execute(frame);
                }
                else
                {
                    RoutedEventHandler loadedHandler = null!;
                    loadedHandler = (s, args) =>
                    {
                        frame.Loaded -= loadedHandler;
                        command.Execute(frame);
                    };
                    frame.Loaded += loadedHandler;
                }
            }
        }
    }
}
