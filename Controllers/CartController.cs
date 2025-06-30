using System.Diagnostics;
using appacd.Models;
using Microsoft.AspNetCore.Mvc;

namespace appacd.Controllers;

public class CartController : Controller
{
    private readonly ILogger<CartController> _logger;

    public CartController(ILogger<CartController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
