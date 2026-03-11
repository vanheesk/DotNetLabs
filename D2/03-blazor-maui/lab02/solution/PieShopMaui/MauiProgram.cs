using Microsoft.Extensions.Logging;
using PieShopMaui.Services;
using PieShopMaui.ViewModels;
using PieShopMaui.Views;

namespace PieShopMaui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// API Service
		builder.Services.AddHttpClient<PieApiService>(client =>
		{
			client.BaseAddress = new Uri("http://localhost:5100");
		});

		// ViewModels
		builder.Services.AddTransient<PieListViewModel>();
		builder.Services.AddTransient<PieDetailViewModel>();
		builder.Services.AddTransient<PieEditViewModel>();

		// Pages
		builder.Services.AddTransient<PieListPage>();
		builder.Services.AddTransient<PieDetailPage>();
		builder.Services.AddTransient<PieEditPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
