using Microsoft.AspNetCore.Mvc;

namespace PieShop.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
