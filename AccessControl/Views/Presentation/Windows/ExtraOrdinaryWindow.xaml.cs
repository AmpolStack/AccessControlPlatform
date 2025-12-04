using AccessControl.Views.Enums;
using System.Windows;
using System.Windows.Input;

namespace AccessControl.Views.Windows
{
    public partial class ExtraOrdinaryWindow : Window
    {
        // DependencyProperty for the element type (Error, Warning, Success, etc.)
        public static readonly DependencyProperty ElementTypeProperty =
            DependencyProperty.Register("ElementType", typeof(AppElementType), typeof(ExtraOrdinaryWindow),
                new PropertyMetadata(AppElementType.Danger));

        // DependencyProperty for the main title
        public static readonly DependencyProperty WindowTitleProperty =
            DependencyProperty.Register("WindowTitle", typeof(string), typeof(ExtraOrdinaryWindow),
                new PropertyMetadata("¡Ups! Ha ocurrido un error inesperado"));

        // DependencyProperty for the message body
        public static readonly DependencyProperty MessageBodyProperty =
            DependencyProperty.Register("MessageBody", typeof(string), typeof(ExtraOrdinaryWindow),
                new PropertyMetadata(string.Empty));

        // DependencyProperty for the button content
        public static readonly DependencyProperty ButtonContentProperty =
            DependencyProperty.Register("ButtonContent", typeof(string), typeof(ExtraOrdinaryWindow),
                new PropertyMetadata("Aceptar"));

        // DependencyProperty for custom content (accepts any XAML)
        public static readonly DependencyProperty CustomContentProperty =
            DependencyProperty.Register("CustomContent", typeof(object), typeof(ExtraOrdinaryWindow),
                new PropertyMetadata(null));

        // DependencyProperty for the button command
        public static readonly DependencyProperty ButtonCommandProperty =
            DependencyProperty.Register("ButtonCommand", typeof(ICommand), typeof(ExtraOrdinaryWindow),
                new PropertyMetadata(null));

        public AppElementType ElementType
        {
            get { return (AppElementType)GetValue(ElementTypeProperty); }
            set { SetValue(ElementTypeProperty, value); }
        }

        public string WindowTitle
        {
            get { return (string)GetValue(WindowTitleProperty); }
            set { SetValue(WindowTitleProperty, value); }
        }

        public string MessageBody
        {
            get { return (string)GetValue(MessageBodyProperty); }
            set { SetValue(MessageBodyProperty, value); }
        }

        public string ButtonContent
        {
            get { return (string)GetValue(ButtonContentProperty); }
            set { SetValue(ButtonContentProperty, value); }
        }

        public object CustomContent
        {
            get { return GetValue(CustomContentProperty); }
            set { SetValue(CustomContentProperty, value); }
        }

        public ICommand ButtonCommand
        {
            get { return (ICommand)GetValue(ButtonCommandProperty); }
            set { SetValue(ButtonCommandProperty, value); }
        }

        public ExtraOrdinaryWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        // Helper method to close the window from the button
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
