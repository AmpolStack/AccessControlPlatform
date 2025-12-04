using System.Windows;
using System.Windows.Controls;

namespace AccessControl.Views.Controls
{
    public partial class HeaderControl : UserControl
    {
        public static readonly DependencyProperty UserNameDependecy =
         DependencyProperty.Register("UserName", typeof(string), typeof(HeaderControl),
             new PropertyMetadata(string.Empty, OnUserNameChanged));

        public static readonly DependencyProperty TruncatedUserNameProperty =
            DependencyProperty.Register("TruncatedUserName", typeof(string), typeof(HeaderControl),
                new PropertyMetadata(string.Empty));

        public HeaderControl()
        {
            InitializeComponent();
        }

        public string UserName
        {
            get { return (string)GetValue(UserNameDependecy); }
            set { SetValue(UserNameDependecy, value); }
        }

        public string TruncatedUserName
        {
            get { return (string)GetValue(TruncatedUserNameProperty); }
            private set { SetValue(TruncatedUserNameProperty, value); }
        }

        private static void OnUserNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is HeaderControl control)
            {
                control.UpdateTruncatedUserName((string)e.NewValue);
            }
        }

        private void UpdateTruncatedUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                TruncatedUserName = string.Empty;
                return;
            }

            if (userName.Length > 30)
            {
                TruncatedUserName = userName.Substring(0, 30) + "...";
            }
            else
            {
                TruncatedUserName = userName;
            }
        }
    }
}
