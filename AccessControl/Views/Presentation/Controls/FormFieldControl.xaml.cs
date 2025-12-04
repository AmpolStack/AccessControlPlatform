using System.Windows;
using System.Windows.Controls;

namespace AccessControl.Views.Controls
{
    public partial class FormFieldControl : UserControl
    {
        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register(
            nameof(LabelText),
            typeof(string),
            typeof(FormFieldControl),
            new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty TextProperty =
          DependencyProperty.Register(
              nameof(Text),
              typeof(string),
              typeof(FormFieldControl),
              new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty InvertedInputStylesProperty =
          DependencyProperty.Register(
          nameof(InvertedInputStyles),
          typeof(bool),
          typeof(FormFieldControl),
          new PropertyMetadata(false));

        public FormFieldControl()
        {
            InitializeComponent();
        }

        public bool InvertedInputStyles
        {
            get => (bool)GetValue(InvertedInputStylesProperty);
            set => SetValue(InvertedInputStylesProperty, value);
        }

        public string LabelText
        {
            get => (string)GetValue(LabelTextProperty);
            set => SetValue(LabelTextProperty, value);
        }


        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }


        public bool IsMultiline
        {
            get => (bool)GetValue(IsMultilineProperty);
            set => SetValue(IsMultilineProperty, value);
        }
        public static readonly DependencyProperty IsMultilineProperty =
            DependencyProperty.Register(nameof(IsMultiline), typeof(bool), typeof(FormFieldControl), new PropertyMetadata(false, OnIsMultilineChanged));

        public TextWrapping TextWrapMode
        {
            get => (TextWrapping)GetValue(TextWrapModeProperty);
            set => SetValue(TextWrapModeProperty, value);
        }
        public static readonly DependencyProperty TextWrapModeProperty =
            DependencyProperty.Register(nameof(TextWrapMode), typeof(TextWrapping), typeof(FormFieldControl), new PropertyMetadata(TextWrapping.NoWrap));

        private static void OnIsMultilineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctl = (FormFieldControl)d;
            ctl.TextWrapMode = (bool)e.NewValue ? TextWrapping.Wrap : TextWrapping.NoWrap;
        }
    }
}
