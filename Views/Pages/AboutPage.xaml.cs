using Wpf.Ui.Controls;

namespace SupernoteDesktopClient.Views.Pages
{
    /// <summary>
    /// Interaction logic for AboutPage.xaml
    /// </summary>
    public partial class AboutPage : INavigableView<ViewModels.AboutViewModel>
    {
        public ViewModels.AboutViewModel ViewModel
        {
            get;
        }

        public AboutPage(ViewModels.AboutViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
        }
    }
}