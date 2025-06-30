using System.Diagnostics;
using appacd.Models;
using Microsoft.AspNetCore.Mvc;

namespace appacd.Controllers;

public class TrackingController : Controller
{
    private readonly ILogger<TrackingController> _logger;

    public TrackingController(ILogger<TrackingController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult OrderStatus()
    {
        return View();
    }

    public IActionResult DetailInvoice()
    {
        return View();
    }
    public IActionResult DetailTeknisi()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
