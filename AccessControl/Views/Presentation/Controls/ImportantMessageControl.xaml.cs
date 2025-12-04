using AccessControl.Views.Enums;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AccessControl.Views.Controls
{
    public partial class ImportantMessageControl : UserControl
    {

        // DependencyProperty for message type (determines color)
        public static readonly DependencyProperty MessageTypeProperty =
         DependencyProperty.Register("MessageType", typeof(AppElementType), typeof(ImportantMessageControl),
             new PropertyMetadata(AppElementType.Sober));

        // DependencyProperty for the title
        public static readonly DependencyProperty TitleContentProperty =
         DependencyProperty.Register("TitleContent", typeof(string), typeof(ImportantMessageControl),
             new PropertyMetadata(string.Empty));

        // DependencyProperty for the message body (plain text)
        public static readonly DependencyProperty BodyContentProperty =
         DependencyProperty.Register("BodyContent", typeof(string), typeof(ImportantMessageControl),
             new PropertyMetadata(string.Empty));

        // DependencyProperty for the button content
        public static readonly DependencyProperty ButtonContentProperty =
         DependencyProperty.Register("ButtonContent", typeof(string), typeof(ImportantMessageControl),
             new PropertyMetadata(string.Empty));

        // DependencyProperty for custom content (accepts any XAML)
        // This eliminates the need to duplicate properties - you can pass any WPF control
        public static readonly DependencyProperty CustomContentProperty =
         DependencyProperty.Register("CustomContent", typeof(object), typeof(ImportantMessageControl),
             new PropertyMetadata(null));

        // DependencyProperty for the button type (you no longer need to duplicate this property of the StandardButtonControl)
        public static readonly DependencyProperty ButtonTypeProperty =
         DependencyProperty.Register("ButtonType", typeof(string), typeof(ImportantMessageControl),
             new PropertyMetadata("Vibrant"));

        // DependencyProperty for the button command
        public static readonly DependencyProperty ButtonCommandProperty =
         DependencyProperty.Register("ButtonCommand", typeof(ICommand), typeof(ImportantMessageControl),
             new PropertyMetadata(null));

        public AppElementType MessageType
        {
            get { return (AppElementType)GetValue(MessageTypeProperty); }
            set { SetValue(MessageTypeProperty, value); }
        }

        public string TitleContent
        {
            get { return (string)GetValue(TitleContentProperty); }
            set { SetValue(TitleContentProperty, value); }
        }

        public string BodyContent
        {
            get { return (string)GetValue(BodyContentProperty); }
            set { SetValue(BodyContentProperty, value); }
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

        public string ButtonType
        {
            get { return (string)GetValue(ButtonTypeProperty); }
            set { SetValue(ButtonTypeProperty, value); }
        }

        public ICommand ButtonCommand
        {
            get { return (ICommand)GetValue(ButtonCommandProperty); }
            set { SetValue(ButtonCommandProperty, value); }
        }

        public ImportantMessageControl()
        {
            InitializeComponent();
        }
    }
}
