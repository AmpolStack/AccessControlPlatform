using AccessControl.Views.ViewModels;
using System.Windows.Controls;

namespace AccessControl.Views.Pages
{
    public partial class EstablishmentPage : Page
    {
        public EstablishmentPage(EstablishmentPageViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
