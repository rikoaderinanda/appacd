using System.Collections.Generic;
using System.Threading.Tasks;
using appacd.Models;
using appacd.Services;
using Microsoft.AspNetCore.Mvc;

namespace appacd.api
{
    [ApiController]
    [Route("api/[controller]")]
    public class PemesananController : ControllerBase
    {
        /// <summary>
        /// Represents a repository interface for managing Layanan (service) entities.
        /// Provides methods for accessing and manipulating Layanan data storage.
        /// </summary>
        private readonly IPemesananRepository _pemesananRepository;

        public PemesananController(IPemesananRepository pemesananRepository)
        {
            _pemesananRepository = pemesananRepository;
        }

        [HttpPost("HapusPemesanan")]
        public async Task<IActionResult> HapusPemesanan([FromBody] HapusPemesananRequest request)
        {
            if (request == null || request.Id <= 0)
            {
                return BadRequest(new { message = "ID tidak valid." });
            }
            try
            {
                var Resid = await _pemesananRepository.HapusPemesananAsync(request.Id);
                return Ok(new { message = "Pemesanan berhasil dihapus", id = Resid });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat menghapus pemesanan", error = ex.Message });
            }
        }

        [HttpPost("SimpanDulu")]
        public async Task<IActionResult> SimpanPemesanan([FromBody] PemesananDto dto)
        {
            if (dto == null || dto.Customer == null)
                return BadRequest(new { message = "Data pemesanan tidak lengkap." });
            try
            {
                var idPemesanan = await _pemesananRepository.SimpanAsync(dto);
                return Ok(new { message = "Pemesanan berhasil disimpan", id = idPemesanan });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat menyimpan pemesanan", error = ex.Message });
            }
        }

        [HttpGet("GetPemesanan")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetPemesanan()
        {
            var pesanan = await _pemesananRepository.GetPemesanan();
            return Ok(pesanan);
        }

        [HttpGet("GetPemesananById")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetPemesananById(string id)
        {
            var pesanan = await _pemesananRepository.GetPemesananById(id);
            return Ok(pesanan);
        }

        [HttpGet("GetKeranjangById")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetKeranjangById(string id)
        {
            var layananList = await _pemesananRepository.GetPemesanan_Keranjang(id);
            return Ok(layananList);
        }
    }
}