using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PieShopMaui.Models;
using PieShopMaui.Services;

namespace PieShopMaui.ViewModels;

public partial class PieListViewModel(PieApiService pieService) : ObservableObject
{
    [ObservableProperty]
    private List<Pie> pies = [];

    [ObservableProperty]
    private bool isLoading;

    [RelayCommand]
    private async Task LoadPiesAsync()
    {
        IsLoading = true;
        Pies = await pieService.GetAllAsync();
        IsLoading = false;
    }

    [RelayCommand]
    private async Task GoToDetailAsync(Pie pie)
    {
        await Shell.Current.GoToAsync($"piedetail?id={pie.PieId}");
    }

    [RelayCommand]
    private async Task GoToAddAsync()
    {
        await Shell.Current.GoToAsync("pieedit");
    }
}
