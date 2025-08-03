using System.Diagnostics;
using appacd.Models;
using appacd.Services;
using Microsoft.AspNetCore.Mvc;

namespace appacd.Controllers;
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public ErrorController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [Route("Error/404")]
    public IActionResult Error404()
    {
        return View("NotFound"); // akan mencari Views/Error/NotFound.cshtml
    }

    [Route("Error/{code}")]
    public IActionResult General(int code)
    {
        return View("General", code); // untuk 403, 500, dll jika perlu
    }
}