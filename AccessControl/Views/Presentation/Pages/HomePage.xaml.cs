using AccessControl.Views.ViewModels;
using System.Windows.Controls;

namespace AccessControl.Views.Pages
{
    public partial class HomePage : Page
    {
        public HomePage(HomePageViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
