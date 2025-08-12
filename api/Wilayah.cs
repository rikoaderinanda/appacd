using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace appacd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WilayahController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public WilayahController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet("provinces")]
        public async Task<IActionResult> GetProvinces()
        {
            var result = await _httpClient.GetStringAsync("https://wilayah.id/api/provinces.json");
            return Content(result, "application/json");
        }

        [HttpGet("regencies/{provinceCode}")]
        public async Task<IActionResult> GetRegencies(string provinceCode)
        {
            var result = await _httpClient.GetStringAsync($"https://wilayah.id/api/regencies/{provinceCode}.json");
            return Content(result, "application/json");
        }

        [HttpGet("districts/{regencyCode}")]
        public async Task<IActionResult> GetDistricts(string regencyCode)
        {
            var result = await _httpClient.GetStringAsync($"https://wilayah.id/api/districts/{regencyCode}.json");
            return Content(result, "application/json");
        }

        [HttpGet("villages/{districtCode}")]
        public async Task<IActionResult> GetVillages(string districtCode)
        {
            var result = await _httpClient.GetStringAsync($"https://wilayah.id/api/villages/{districtCode}.json");
            return Content(result, "application/json");
        }

    }
}
