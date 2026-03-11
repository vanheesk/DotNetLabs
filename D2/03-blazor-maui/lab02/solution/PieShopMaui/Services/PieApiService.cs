using System.Net.Http.Json;
using PieShopMaui.Models;

namespace PieShopMaui.Services;

public class PieApiService(HttpClient http)
{
    public async Task<List<Pie>> GetAllAsync()
        => await http.GetFromJsonAsync<List<Pie>>("pies") ?? [];

    public async Task<Pie?> GetByIdAsync(int id)
        => await http.GetFromJsonAsync<Pie>($"pies/{id}");

    public async Task<Pie?> CreateAsync(Pie pie)
    {
        var response = await http.PostAsJsonAsync("pies", pie);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Pie>();
    }

    public async Task<Pie?> UpdateAsync(int id, Pie pie)
    {
        var response = await http.PutAsJsonAsync($"pies/{id}", pie);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<Pie>();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await http.DeleteAsync($"pies/{id}");
        return response.IsSuccessStatusCode;
    }
}
