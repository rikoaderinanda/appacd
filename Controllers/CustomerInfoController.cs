using System.Diagnostics;
using app.acd.Models;
using Microsoft.AspNetCore.Mvc;

namespace app.acd.Controllers;

public class CustomerInfoController : Controller
{
    private readonly ILogger<CustomerInfoController> _logger;

    public CustomerInfoController(ILogger<CustomerInfoController> logger)
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
