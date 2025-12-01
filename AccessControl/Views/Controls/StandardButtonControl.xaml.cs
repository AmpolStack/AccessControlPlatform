using AccessControl.Views.Enums;
using System.Windows;
using System.Windows.Controls;

namespace AccessControl.Views.Controls
{
    public partial class StandardButtonControl : UserControl
    {

        public static readonly DependencyProperty ButtonTypeProperty =
         DependencyProperty.Register("ButtonType", typeof(ButtonType), typeof(StandardButtonControl),
             new PropertyMetadata(ButtonType.Sober));

        public static readonly DependencyProperty ButtonContentProperty =
         DependencyProperty.Register("ButtonContent", typeof(string), typeof(StandardButtonControl),
             new PropertyMetadata(string.Empty));

        public StandardButtonControl()
        {
            InitializeComponent();
        }

        public ButtonType ButtonType
        {
            get { return (ButtonType)GetValue(ButtonTypeProperty); }
            set { SetValue(ButtonTypeProperty, value); }
        }

        public string ButtonContent
        {
            get { return (string)GetValue(ButtonContentProperty); }
            set { SetValue(ButtonContentProperty, value); }
        }
    }
}
