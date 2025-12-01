using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AccessControl.Views.Controls
{
    public partial class HeaderControl : UserControl
    {
        public readonly DependencyProperty UserNameDependecy =
         DependencyProperty.Register("UserName", typeof(string), typeof(HeaderControl),
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
    }
}
