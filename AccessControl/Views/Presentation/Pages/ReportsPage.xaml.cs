using AccessControl.Views.ViewModels;
using System.Windows.Controls;

namespace AccessControl.Views.Pages
{
    public partial class ReportsPage : Page
    {
        public ReportsPage(ReportsPageViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
