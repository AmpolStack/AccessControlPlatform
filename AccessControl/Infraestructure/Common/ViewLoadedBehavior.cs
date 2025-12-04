using System.Windows;
using System.Windows.Input;

namespace AccessControl.Infraestructure.Common
{
    class ViewLoadedBehavior
    {
        public static readonly DependencyProperty OnLoadedProperty =
            DependencyProperty.RegisterAttached(
                "OnLoaded", typeof(ICommand), typeof(ViewLoadedBehavior),
                new PropertyMetadata(null, OnLoadedChanged));

        public static void SetOnLoaded(DependencyObject obj, ICommand value)
            => obj.SetValue(OnLoadedProperty, value);

        public static ICommand GetOnLoaded(DependencyObject obj)
            => (ICommand)obj.GetValue(OnLoadedProperty);

        private static void OnLoadedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element)
            {
                element.Loaded += (s, ev) =>
                {
                    ICommand cmd = GetOnLoaded(element);
                    if (cmd?.CanExecute(null) == true)
                        cmd.Execute(null);
                };
            }
        }
    }
}
