using PieShopMaui.ViewModels;

namespace PieShopMaui.Views;

public partial class PieListPage : ContentPage
{
    private readonly PieListViewModel _viewModel;

    public PieListPage(PieListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadPiesCommand.ExecuteAsync(null);
    }
}
