using System.Diagnostics;
using appacd.Models;
using Microsoft.AspNetCore.Mvc;
using appacd.Services;

namespace appacd.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class LayananController : Controller
    {
        private readonly ILogger<LayananController> _logger;
        private readonly ILayananRepository _layananRepository;

        public LayananController(ILogger<LayananController> logger, ILayananRepository layananRepository)
        {
            _logger = logger;
            _layananRepository = layananRepository;
        }

        [Route("Layanan/{id}")]
        public async Task<IActionResult> Index(string id)
        {
            var layanan = await _layananRepository.GetLayananById(id);
            foreach (var d in layanan)
            {
                ViewData["NamaLayanan"] = d.nama_layanan;
                ViewData["IdLayanan"] = d.id;
            }
            // ViewData["NamaLayanan"] = layanan?.nama_layanan ?? "Layanan Tidak Ditemukan";
            ViewData["Id"] = id;
            return View(layanan);
        }

        [Route("Layanan/CuciAC")]
        public async Task<IActionResult> CuciAC()
        {
            ViewData["Id"] = 2;
            return View();
        }

        [Route("Layanan/Perbaikan")]
        public async Task<IActionResult> Perbaikan()
        {
            ViewData["Id"] = 7;
            return View();
        }

        [Route("Layanan/Relokasi")]
        public async Task<IActionResult> Relokasi()
        {
            ViewData["Id"] = 13;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
