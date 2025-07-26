using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using appacd.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text.Json;

namespace appacd.Services
{

    public interface ITripayRepository
    {
        Task<dynamic> GetPaymentChannelsAsync();
    }

    public class TripayRepository : ITripayRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "DEV-FPdSS20exLUIs3d7A3NYpUBtbkOtrVMVCUlhsyT8"; // Ganti sesuai data kamu
        private readonly IDbConnection _db;

        public TripayRepository(HttpClient httpClient,IDbConnection db)
        {
            _db = db;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://tripay.co.id/api-sandbox/");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        public async Task<dynamic> GetPaymentChannelsAsync()
        {
            var response = await _httpClient.GetAsync("payment/channel");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var obj = JObject.Parse(json);

            return obj["data"]; // list of channel objects (dynamic)
        }

    }

    public class TripayChannelResponse
    {
        public bool Success { get; set; }
        public List<Channel> Data { get; set; }
    }
}