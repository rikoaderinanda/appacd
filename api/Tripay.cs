using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using appacd.Models;
using appacd.Services;
using System.Text.Json;
using System.Text;
namespace appacd.api
{
    [ApiController]
    [Route("api/[controller]")]
    public class TripayController : ControllerBase
    {
        private readonly ITripayRepository _repo;

        public TripayController(ITripayRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("GetPaymentChannels")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetPaymentChannels()
        {
            var res = await _repo.GetPaymentChannelsAsync();
            return Ok(res);
        }

        [HttpGet("GetPaymentChannelsV2")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetPaymentChannelsV2()
        {
            var res = await _repo.GetPaymentChannelsV2Async();
            return Ok(res);
        }

        [HttpGet("create-signature")]
        public async Task<ActionResult<dynamic>> CreateSignature(string noReferensiMerchant,decimal nominal)
        {
            if (string.IsNullOrWhiteSpace(noReferensiMerchant) || nominal <= 0)
                return BadRequest("Referensi atau nominal tidak valid");
            var res = await _repo.GetSignatureAsync(noReferensiMerchant, nominal);
            return Ok(res);
        }
       
        [HttpPost("CreateTransaction")]
        public async Task<IActionResult> CreateTransaction([FromBody] TripayTransactionRequest dt)
        {
            if (dt == null)
                return BadRequest("Invalid request body");
            if (string.IsNullOrEmpty(dt.merchant_ref) || dt.amount <= 0)
                return BadRequest("MerchantRef dan Amount wajib diisi");
            
            try
            {
                var result = await _repo.CreateTransactionAsync(dt);
                return Ok(result);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(502, $"Tripay API error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}