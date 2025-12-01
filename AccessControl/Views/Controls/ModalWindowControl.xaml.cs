using AccessControl.Views.Enums;
using System.Windows;
using System.Windows.Controls;

namespace AccessControl.Views.Controls
{
    public partial class ModalWindowControl : UserControl
    {
        public static readonly DependencyProperty ModalWindowTypeProperty =
        DependencyProperty.Register("ModalWindowType", typeof(AppElementType), typeof(ModalWindowControl),
          new PropertyMetadata(AppElementType.Sober));

        public static readonly DependencyProperty ModalWindowVisibilityProperty =
        DependencyProperty.Register("ModalWindowVisibility", typeof(bool), typeof(ModalWindowControl),
          new PropertyMetadata(true));

        public static readonly DependencyProperty ModalWindowTextProperty =
       DependencyProperty.Register("ModalWindowText", typeof(string), typeof(ModalWindowControl),
         new PropertyMetadata(string.Empty));

        public ModalWindowControl()
        {
            InitializeComponent();
        }

        public AppElementType ModalWindowType
        {
            get { return (AppElementType)GetValue(ModalWindowTypeProperty); }
            set { SetValue(ModalWindowTypeProperty, value); }
        }

        public bool ModalWindowVisibility
        {
            get { return (bool)GetValue(ModalWindowVisibilityProperty); }
            set { SetValue(ModalWindowVisibilityProperty, value); }
        }

        public string ModalWindowText
        {
            get { return (string)GetValue(ModalWindowTextProperty); }
            set { SetValue(ModalWindowTextProperty, value); }
        }

    }
}
