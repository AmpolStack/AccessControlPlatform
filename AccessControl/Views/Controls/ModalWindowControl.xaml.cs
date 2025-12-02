using AccessControl.Views.Enums;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AccessControl.Views.Controls
{
    public partial class ModalWindowControl : UserControl
    {
        public static readonly DependencyProperty ModalWindowTypeProperty =
        DependencyProperty.Register("ModalWindowType", typeof(AppElementType), typeof(ModalWindowControl),
          new PropertyMetadata(AppElementType.Sober));

        public static readonly DependencyProperty ModalWindowVisibilityProperty =
            DependencyProperty.Register("ModalWindowVisibility", typeof(bool), typeof(ModalWindowControl),
              new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnVisibilityChanged));

        public static readonly DependencyProperty ModalWindowTextProperty =
           DependencyProperty.Register("ModalWindowText", typeof(string), typeof(ModalWindowControl),
             new PropertyMetadata(string.Empty));

        public ModalWindowControl()
        {
            InitializeComponent();
            // Inicializar estado
            if (MainBorder != null)
            {
                MainBorder.Visibility = Visibility.Collapsed;
                // Posición inicial fuera de pantalla (derecha)
                if (MainBorder.RenderTransform is TranslateTransform transform)
                {
                    transform.X = 400;
                }
            }
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

        private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ModalWindowControl control && control.MainBorder != null)
            {
                bool isVisible = (bool)e.NewValue;
                control.AnimateModal(isVisible);
            }
        }

        private void AnimateModal(bool show)
        {
            if (show)
            {
                MainBorder.Visibility = Visibility.Visible;
                
                // Animación de entrada: De derecha (400) a 0
                var slideIn = new System.Windows.Media.Animation.DoubleAnimation
                {
                    From = 400,
                    To = 0,
                    Duration = new Duration(TimeSpan.FromSeconds(0.3)),
                    EasingFunction = new System.Windows.Media.Animation.CubicEase { EasingMode = System.Windows.Media.Animation.EasingMode.EaseOut }
                };
                
                if (MainBorder.RenderTransform is TranslateTransform transform)
                {
                    transform.BeginAnimation(TranslateTransform.XProperty, slideIn);
                }
            }
            else
            {
                // Animación de salida: De 0 a derecha (400)
                var slideOut = new System.Windows.Media.Animation.DoubleAnimation
                {
                    From = 0,
                    To = 400,
                    Duration = new Duration(TimeSpan.FromSeconds(0.3)),
                    EasingFunction = new System.Windows.Media.Animation.CubicEase { EasingMode = System.Windows.Media.Animation.EasingMode.EaseIn }
                };

                slideOut.Completed += (s, e) =>
                {
                    MainBorder.Visibility = Visibility.Collapsed;
                };

                if (MainBorder.RenderTransform is TranslateTransform transform)
                {
                    transform.BeginAnimation(TranslateTransform.XProperty, slideOut);
                }
            }
        }

        private void CloseModal_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ModalWindowVisibility = false;
        }
    }
}
