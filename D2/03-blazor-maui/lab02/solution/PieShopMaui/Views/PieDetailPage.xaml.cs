using PieShopMaui.ViewModels;

namespace PieShopMaui.Views;

public partial class PieDetailPage : ContentPage
{
    public PieDetailPage(PieDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
