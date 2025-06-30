using System.Diagnostics;
using appacd.Models;
using Microsoft.AspNetCore.Mvc;

namespace appacd.Controllers;

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
