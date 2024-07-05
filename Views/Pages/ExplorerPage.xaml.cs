using Wpf.Ui.Controls;

namespace SupernoteDesktopClient.Views.Pages
{
    /// <summary>
    /// Interaction logic for ExplorerPage.xaml
    /// </summary>
    public partial class ExplorerPage : INavigableView<ViewModels.ExplorerViewModel>
    {
        public ViewModels.ExplorerViewModel ViewModel
        {
            get;
        }

        public ExplorerPage(ViewModels.ExplorerViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
        }
    }
}