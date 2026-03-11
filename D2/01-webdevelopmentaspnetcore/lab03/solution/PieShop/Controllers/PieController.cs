using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PieShop.Data;
using PieShop.Models;
using PieShop.Services;
using PieShop.ViewModels;

namespace PieShop.Controllers;

public class PieController : Controller
{
    private readonly IPieRepository _pieRepository;
    private readonly PieShopDbContext _context;
    private readonly InviteLinkService _inviteLinkService;

    public PieController(IPieRepository pieRepository, PieShopDbContext context, InviteLinkService inviteLinkService)
    {
        _pieRepository = pieRepository;
        _context = context;
        _inviteLinkService = inviteLinkService;
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

    // CRUD: Create
    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Categories = _context.Categories.ToList();
        return View();
    }

    [HttpPost]
    public IActionResult Create(Pie pie)
    {
        if (ModelState.IsValid)
        {
            _context.Pies.Add(pie);
            _context.SaveChanges();
            return RedirectToAction("List");
        }
        ViewBag.Categories = _context.Categories.ToList();
        return View(pie);
    }

    // CRUD: Edit
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var pie = _context.Pies.Find(id);
        if (pie == null) return NotFound();
        ViewBag.Categories = _context.Categories.ToList();
        return View(pie);
    }

    [HttpPost]
    public IActionResult Edit(Pie pie)
    {
        if (ModelState.IsValid)
        {
            _context.Pies.Update(pie);
            _context.SaveChanges();
            return RedirectToAction("List");
        }
        ViewBag.Categories = _context.Categories.ToList();
        return View(pie);
    }

    // CRUD: Delete
    [HttpPost]
    public IActionResult Delete(int id)
    {
        var pie = _context.Pies.Find(id);
        if (pie != null)
        {
            _context.Pies.Remove(pie);
            _context.SaveChanges();
        }
        return RedirectToAction("List");
    }

    // Data shaping: Search with filtering, sorting, pagination
    public IActionResult Search(string? category, decimal? maxPrice, string? sortBy, int page = 1, int pageSize = 5)
    {
        var query = _context.Pies.Include(p => p.Category).AsQueryable();

        if (!string.IsNullOrEmpty(category))
            query = query.Where(p => p.Category!.Name == category);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice.Value);

        // Sorting
        query = sortBy switch
        {
            "price" => query.OrderBy(p => p.Price),
            "name" => query.OrderBy(p => p.Name),
            _ => query.OrderBy(p => p.PieId)
        };

        // Stats
        var totalCount = query.Count();
        var avgPrice = query.Any() ? query.Average(p => p.Price) : 0;

        // Pagination
        var pies = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        ViewBag.TotalCount = totalCount;
        ViewBag.AveragePrice = avgPrice;
        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.Category = category;
        ViewBag.MaxPrice = maxPrice;
        ViewBag.SortBy = sortBy;
        ViewBag.Categories = _context.Categories.Select(c => c.Name).ToList();

        return View(pies);
    }

    // Programmatic URL generation
    public IActionResult ShareLink(int id)
    {
        var link = _inviteLinkService.GeneratePieLink(id);
        return Ok(new { shareUrl = link });
    }
}
