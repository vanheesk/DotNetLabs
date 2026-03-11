using PieShopMaui.ViewModels;

namespace PieShopMaui.Views;

public partial class PieEditPage : ContentPage
{
    public PieEditPage(PieEditViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
