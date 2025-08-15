using System.Collections.Generic;
using System.Threading.Tasks;
using appacd.Models;
using appacd.Services;
using Microsoft.AspNetCore.Mvc;

namespace appacd.api
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "transaksi")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionRepo _repo;
        public TransactionController(ITransactionRepo repo)
        {
            _repo = repo;
        }

        [HttpPost("SimpanDulu")]
        public async Task<IActionResult> SimpanDulu([FromBody] LogTransaction dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Data pemesanan tidak lengkap." });
            try
            {
                var idPemesanan = await _repo.SimpanAsync(dto);
                return Ok(new { message = "Pemesanan berhasil disimpan", id = idPemesanan });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat menyimpan pemesanan", error = ex.Message });
            }
        }

        [HttpGet("GetCart_ByUserId")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetCart_ByUserId(string id)
        {
            var res = await _repo.GetCart_ByUserId(id);
            return Ok(res);
        }

        [HttpGet("GetDetailTrx_ById")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetDetailTrx_ById(string id)
        {
            var res = await _repo.GetDetail_ById(id);
            return Ok(res);
        }


        [HttpDelete("DeleteCart/{id}")]
        public async Task<IActionResult> DeleteCart([FromRoute] string id)
        {
            try
            {
                var res = await _repo.DeleteCart(id);
                return Ok(new { message = "Delete data berhasil", success = res });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan", error = ex.Message });
            }
        }
    }
}