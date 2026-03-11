using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PieShopMaui.Models;
using PieShopMaui.Services;

namespace PieShopMaui.ViewModels;

[QueryProperty(nameof(PieId), "id")]
public partial class PieDetailViewModel(PieApiService pieService) : ObservableObject
{
    [ObservableProperty]
    private int pieId;

    [ObservableProperty]
    private Pie? pie;

    partial void OnPieIdChanged(int value)
    {
        Task.Run(() => LoadPieAsync(value));
    }

    private async Task LoadPieAsync(int id)
    {
        Pie = await pieService.GetByIdAsync(id);
    }

    [RelayCommand]
    private async Task EditAsync()
    {
        await Shell.Current.GoToAsync($"pieedit?id={PieId}");
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        bool confirm = await Shell.Current.DisplayAlertAsync(
            "Delete Pie", $"Are you sure you want to delete {Pie?.Name}?", "Yes", "No");

        if (confirm)
        {
            await pieService.DeleteAsync(PieId);
            await Shell.Current.GoToAsync("..");
        }
    }
}
