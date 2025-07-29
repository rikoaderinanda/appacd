using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using appacd.Models;
using appacd.Services;
using System.Text.Json;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Primitives;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace appacd.api
{
    [ApiController]
    [Route("api/[controller]")]
    public class TripayController : ControllerBase
    {
        private readonly ITripayRepository _repo;
        private readonly IPemesananRepository _pesanan;
        private readonly ILogger<TripayController> _logger;

        public TripayController(ITripayRepository repo, IPemesananRepository pesanan, ILogger<TripayController> logger)
        {
            _repo = repo;
            _pesanan = pesanan;
            _logger = logger;
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

        [HttpGet("GetDetailTransaction")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetDetailTransaction(string reference)
        {
            var res = await _repo.GetDetailTransactionAsync(reference);
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

        [HttpPost("create-signature-callback")]
        public async Task<ActionResult<dynamic>> CreateSignatureCallback([FromBody] TripayCallbackDto dto)
        {
            string json = JsonConvert.SerializeObject(dto, Formatting.None);
            var res = await _repo.GetSignatureCallbackAsync(json);
            return Ok(res);
        }

        [HttpGet("GetInvoiceByReference")]
        public async Task<ActionResult<dynamic>> GetInvoiceByReference(string reference)
        {
            var res = await _pesanan.GetInvoiceByReference(reference);
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

        [HttpPost("callback-transaction")]
        [SwaggerOperation(Summary = "Callback from Tripay", Description = "Handles Tripay payment status")]
        public async Task<IActionResult> Callback(
            [FromHeader(Name = "X-Callback-Signature")] string callbackSignature,
            [FromHeader(Name = "X-Callback-Event")] string callbackEvent,
            [FromBody] TripayCallbackDto body)
        {
            // --- BUFFERING AGAR Body bisa dibaca 2x ---
            Request.EnableBuffering();

            // Signature validation
            string json = JsonConvert.SerializeObject(body, Formatting.None);
            var expectedSignature = await _repo.GetSignatureCallbackAsync(json);

            if (callbackSignature != expectedSignature)
                return BadRequest(new { success = false, message = "Invalid signature" });

            if (callbackEvent != "payment_status")
                return BadRequest(new { success = false, message = $"Unexpected event: {callbackEvent}" });


            var resInv = await _pesanan.GetInvoiceByReference(body.reference);

            if (resInv == null || resInv.Count == 0)
            {
                return BadRequest(new { success = false, message = "Invalid Reference" });
            }

            var resUpdateInv = await _pesanan.UpdateInvoiceStatusAsync(resInv[0].id.ToString(), body.status);
            if (!resUpdateInv)
            {
                return BadRequest(new { success = false, message = $"update gagal" });
            }

            _logger.LogInformation("Callback: Ref={Ref}, Status={Status}", body.reference, body.status);

            // // Update invoice status, etc
            return Ok(new { success = resUpdateInv });
        }
    }
}