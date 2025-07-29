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

            string requestBody;

            using (var reader = new StreamReader(Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true))
            {
                requestBody = await reader.ReadToEndAsync();
                Request.Body.Position = 0; // Reset posisi stream supaya bisa dibaca lagi oleh model binder
            }
            // Signature validation
            var expectedSignature = await _repo.GetSignatureCallbackAsync(requestBody);
            if (callbackSignature != expectedSignature)
                return BadRequest(new { success = false, message = "Invalid signature" });

            if (callbackEvent != "payment_status")
                return BadRequest(new { success = false, message = $"Unexpected event: {callbackEvent}" });

            _logger.LogInformation("Callback: Ref={Ref}, Status={Status}", body.reference, body.status);

            // // Update invoice status, etc
            return Ok(new { success = true });
        }
    }
}