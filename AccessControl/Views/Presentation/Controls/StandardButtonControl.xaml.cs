using AccessControl.Views.Enums;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AccessControl.Views.Controls
{
    public partial class StandardButtonControl : UserControl
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(StandardButtonControl),
                new PropertyMetadata(null));

        public static readonly DependencyProperty ButtonTypeProperty =
         DependencyProperty.Register("ButtonType", typeof(AppElementType), typeof(StandardButtonControl),
             new PropertyMetadata(AppElementType.Sober));

        public static readonly DependencyProperty ButtonContentProperty =
         DependencyProperty.Register("ButtonContent", typeof(string), typeof(StandardButtonControl),
             new PropertyMetadata(string.Empty));

        public StandardButtonControl()
        {
            InitializeComponent();
        }

        public AppElementType ButtonType
        {
            get { return (AppElementType)GetValue(ButtonTypeProperty); }
            set { SetValue(ButtonTypeProperty, value); }
        }

        public string ButtonContent
        {
            get { return (string)GetValue(ButtonContentProperty); }
            set { SetValue(ButtonContentProperty, value); }
        }

        public ICommand? Command
        {
            get => (ICommand?)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(StandardButtonControl), new PropertyMetadata(null));

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }
    }
}
