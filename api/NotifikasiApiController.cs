using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using appacd.Hubs;

namespace appacd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotifikasiController : ControllerBase
    {
        private readonly IHubContext<NotifikasiHub> _hubContext;

        public NotifikasiController(IHubContext<NotifikasiHub> hubContext)
        {
            _hubContext = hubContext;
        }

        /// <summary>
        /// Broadcast ke semua user
        /// </summary>
        [HttpPost("broadcast")]
        public async Task<IActionResult> Broadcast([FromBody] string pesan)
        {
            await _hubContext.Clients.All.SendAsync("TerimaNotifikasi", pesan);
            return Ok(new { success = true, message = "Broadcast sent" });
        }

        /// <summary>
        /// Kirim personal ke user tertentu (by userId dari JWT nameidentifier)
        /// </summary>
        [HttpPost("Update_StatusTrackingOrder")]
        public async Task<IActionResult> Update_StatusTrackingOrder([FromQuery] string userId, [FromBody] string pesan)
        {
            await _hubContext.Clients.User(userId).SendAsync("UpdateStatusTrackingOrder", pesan);
            return Ok(new { success = true, message = $"Pesan dikirim ke user {userId}" });
        }

        [HttpPost("Update_PembayaranSukses")]
        public async Task<IActionResult> Update_PembayaranSukses([FromQuery] string userId, [FromBody] string pesan)
        {
            await _hubContext.Clients.User(userId).SendAsync("UpdatePembayaranSukses", pesan);
            return Ok(new { success = true, message = $"Pesan dikirim ke user {userId}" });
        }
    }
}
