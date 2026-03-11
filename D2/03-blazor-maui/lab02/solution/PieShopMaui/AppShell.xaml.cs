using PieShopMaui.Views;

namespace PieShopMaui;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute("piedetail", typeof(PieDetailPage));
		Routing.RegisterRoute("pieedit", typeof(PieEditPage));
	}
}
