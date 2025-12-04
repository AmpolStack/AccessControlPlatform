using AccessControl.Views.ViewModels;
using System.Windows.Controls;

namespace AccessControl.Views.Pages
{
    public partial class ProfilePage : Page
    {
        public ProfilePage(ProfilePageViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
