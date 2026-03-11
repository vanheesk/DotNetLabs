using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PieShopMaui.Models;
using PieShopMaui.Services;

namespace PieShopMaui.ViewModels;

[QueryProperty(nameof(PieId), "id")]
public partial class PieEditViewModel(PieApiService pieService) : ObservableObject
{
    [ObservableProperty]
    private int pieId;

    [ObservableProperty]
    private string name = "";

    [ObservableProperty]
    private string shortDescription = "";

    [ObservableProperty]
    private decimal price;

    [ObservableProperty]
    private bool isPieOfTheWeek;

    [ObservableProperty]
    private int categoryId = 1;

    [ObservableProperty]
    private string pageTitle = "Add a New Pie";

    public bool IsEditing => PieId > 0;

    partial void OnPieIdChanged(int value)
    {
        if (value > 0)
        {
            PageTitle = "Edit Pie";
            Task.Run(() => LoadPieAsync(value));
        }
    }

    private async Task LoadPieAsync(int id)
    {
        var pie = await pieService.GetByIdAsync(id);
        if (pie is null) return;

        Name = pie.Name;
        ShortDescription = pie.ShortDescription ?? "";
        Price = pie.Price;
        IsPieOfTheWeek = pie.IsPieOfTheWeek;
        CategoryId = pie.CategoryId;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            await Shell.Current.DisplayAlertAsync("Validation", "Name is required.", "OK");
            return;
        }

        var pie = new Pie
        {
            PieId = PieId,
            Name = Name,
            ShortDescription = ShortDescription,
            Price = Price,
            IsPieOfTheWeek = IsPieOfTheWeek,
            CategoryId = CategoryId
        };

        if (IsEditing)
        {
            await pieService.UpdateAsync(PieId, pie);
        }
        else
        {
            await pieService.CreateAsync(pie);
        }

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
