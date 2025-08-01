using System.Diagnostics;
using appacd.Models;
using appacd.Services;
using Microsoft.AspNetCore.Mvc;

namespace appacd.Controllers;

public class HomeController : Controller
{
    private readonly ILayananRepository _repo;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, ILayananRepository repo)
    {
        _logger = logger;
        _repo = repo;
    }

    public IActionResult Index()
    {
        //var data = await _repo.GetAllAsync();
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Profile()
    {
        return View();
    }

    public IActionResult manageaddress()
    {
        return View();
    }

    public IActionResult HistoryPerangkat()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
