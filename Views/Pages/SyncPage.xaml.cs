using Wpf.Ui.Controls;

namespace SupernoteDesktopClient.Views.Pages
{
    /// <summary>
    /// Interaction logic for SyncPage.xaml
    /// </summary>
    public partial class SyncPage : INavigableView<ViewModels.SyncViewModel>
    {
        public ViewModels.SyncViewModel ViewModel
        {
            get;
        }

        public SyncPage(ViewModels.SyncViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
        }
    }
}
