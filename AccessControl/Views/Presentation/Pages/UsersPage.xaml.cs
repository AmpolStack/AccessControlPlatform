using AccessControl.Views.ViewModels;
using System.Windows.Controls;

namespace AccessControl.Views.Pages
{
    public partial class UsersPage : Page
    {
        private readonly UsersPageViewModel _viewModel;

        public UsersPage(UsersPageViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
    }
}
