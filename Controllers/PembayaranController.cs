using System.Diagnostics;
using app.acd.Models;
using Microsoft.AspNetCore.Mvc;

namespace app.acd.Controllers;

public class PembayaranController : Controller
{
    private readonly ILogger<PembayaranController> _logger;

    public PembayaranController(ILogger<PembayaranController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult CallApiPaymentGateway()
    {
        return View();
    }

    public IActionResult Status()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
