using Microsoft.AspNetCore.Mvc;
using PieShop.Models;
using PieShop.ViewModels;

namespace PieShop.Controllers;

public class PieController : Controller
{
    private readonly IPieRepository _pieRepository;

    public PieController(IPieRepository pieRepository)
    {
        _pieRepository = pieRepository;
    }

    public IActionResult List()
    {
        var viewModel = new PiesListViewModel
        {
            Pies = _pieRepository.AllPies,
            CurrentCategory = "All Pies"
        };
        return View(viewModel);
    }

    public IActionResult Details(int id)
    {
        var pie = _pieRepository.GetPieById(id);
        if (pie == null)
            return NotFound();
        return View(pie);
    }

    public IActionResult ListJson()
    {
        return Json(_pieRepository.AllPies);
    }
}
