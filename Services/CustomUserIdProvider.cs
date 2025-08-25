using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace appacd.Services
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            // Ambil userId dari JWT payload (claim "nameidentifier")
            var userId = connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            Console.WriteLine($"🔑 SignalR resolved UserId: {userId}");
            return userId;
        }
    }
}
