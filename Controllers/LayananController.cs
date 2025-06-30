using System.Diagnostics;
using app.acd.Models;
using Microsoft.AspNetCore.Mvc;

namespace app.acd.Controllers;

public class LayananController : Controller
{
    private readonly ILogger<LayananController> _logger;

    public LayananController(ILogger<LayananController> logger)
    {
        _logger = logger;
    }

    [Route("Layanan/{NamaLayanan}")]
    public IActionResult Index(string NamaLayanan)
    {
        ViewData["NamaLayanan"] = NamaLayanan;
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
