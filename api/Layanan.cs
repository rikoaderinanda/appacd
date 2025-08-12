using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using appacd.Services;
using appacd.Models;

namespace appacd.api
{
    [ApiController]
    [Route("api/[controller]")]
    public class LayananController : ControllerBase
    {
        /// <summary>
        /// Represents a repository interface for managing Layanan (service) entities.
        /// Provides methods for accessing and manipulating Layanan data storage.
        /// </summary>
        private readonly ILayananRepository _layananRepository;
        private readonly IAccountRepository _accRepo;

        public LayananController(ILayananRepository layananRepository, IAccountRepository accRepo)
        {
            _layananRepository = layananRepository;
            _accRepo = accRepo;
        }

        [HttpGet("GetLayanan")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetAll()
        {
            var layananList = await _layananRepository.GetAllAsync();
            return Ok(layananList);
        }

        [HttpGet("GetLayananById")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetLayananById(string Id)
        {
            var layananList = await _layananRepository.GetLayananById(Id);
            return Ok(layananList);
        }

        [HttpGet("InfoLayanan")]
        public async Task<ActionResult<IEnumerable<dynamic>>> InfoLayanan(string Id)
        {
            var layananList = await _layananRepository.InfoLayananAsync(Id);
            return Ok(layananList);
        }

        [HttpGet("KeluhanMasalah")]
        public async Task<ActionResult<IEnumerable<dynamic>>> KeluhanMasalah(string Id)
        {
            var layananList = await _layananRepository.KeluhanMasalahAsync(Id);
            return Ok(layananList);
        }

        [HttpGet("JasaLayanan")]
        public async Task<ActionResult<IEnumerable<dynamic>>> JasaLayanan(string Id)
        {
            var layananList = await _layananRepository.JasaLayananAsync(Id);
            return Ok(layananList);
        }

        [HttpGet("JenisProperti")]
        public async Task<ActionResult<IEnumerable<dynamic>>> JenisProperti(string Id)
        {
            var layananList = await _layananRepository.JenisPropertiAsync(Id);
            return Ok(layananList);
        }

        [HttpGet("BannerLayanan")]
        public async Task<ActionResult<IEnumerable<dynamic>>> BannerLayanan(string Id)
        {
            var layananList = await _layananRepository.BannerLayananAsync(Id);
            return Ok(layananList);
        }

        [HttpGet("JasaLayananDetail")]
        public async Task<ActionResult<IEnumerable<dynamic>>> JasaLayananDetail(string Id)
        {
            var layananList = await _layananRepository.JasaLayananDetailAsync(Id);
            return Ok(layananList);
        }

        [HttpGet("PaketLangganan")]
        public async Task<ActionResult<IEnumerable<dynamic>>> PaketLangganan(string Id)
        {
            var layananList = await _layananRepository.LanggananJasaAsync(Id);
            return Ok(layananList);
        }

        [HttpGet("ListKontakUser")]
        public async Task<ActionResult<IEnumerable<dynamic>>> ListKontakUser(string Id)
        {
            var layananList = await _accRepo.GetListKontakAsync(Id);
            return Ok(layananList);
        }

        [HttpPost("SimpanKontak")]
        public async Task<IActionResult> SimpanKontak([FromBody] Kontak request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "model tidak valid." });
            }
            try
            {
                var Resid = await _accRepo.SimpanKontakAsync(request);
                if (Resid)
                {
                    return Ok(new { message = "Simpan Kontak berhasil", success = Resid });
                }
                return Ok(new { message = "Data kontak already exist", success = Resid });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat menghapus pemesanan", success = false, error = ex.Message });
            }
        }

        [HttpDelete("DeleteKontak/{id}")]
        public async Task<IActionResult> DeleteKontak([FromRoute] string id)
        {
            try
            {
                var res = await _accRepo.DeleteKontak(id);
                return Ok(new { message = "Delete data berhasil", success = res });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan", error = ex.Message });
            }
        }

        [HttpGet("ListAlamat")]
        public async Task<ActionResult<IEnumerable<dynamic>>> ListAlamat(string Id)
        {
            var layananList = await _accRepo.GetAlamatAsync(Id);
            return Ok(layananList);
        }
    }
}