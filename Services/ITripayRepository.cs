using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using appacd.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace appacd.Services
{

    public interface ITripayRepository
    {
        Task<dynamic> GetPaymentChannelsAsync();
        Task<dynamic> GetPaymentChannelsV2Async();
        Task<string>  GetSignatureAsync(string noReferensiMerchant, decimal nominal);
        Task<dynamic> CreateTransactionAsync(TripayTransactionRequest dt);
    }

    public class TripayRepository : ITripayRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _privateKey; // Ganti sesuai data kamu
        private readonly string _kodeMerchant;
        private readonly IDbConnection _db;

        public TripayRepository(HttpClient httpClient, IDbConnection db, IConfiguration config)
        {
            _db = db;
            _httpClient = httpClient;

            var tripayConfig = config.GetSection("Tripay");
            _apiKey = tripayConfig["ApiKey"];
            _privateKey = tripayConfig["PrivateKey"];
            _kodeMerchant = tripayConfig["MerchantCode"];
            _httpClient.BaseAddress = new Uri(tripayConfig["BaseAddress"]);
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

        public async Task<dynamic> GetPaymentChannelsV2Async()
        {
            var response = await _httpClient.GetAsync("merchant/payment-channel");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var obj = JObject.Parse(json);
            return obj["data"];
        }

        public async Task<dynamic> CreateTransactionAsync(TripayTransactionRequest dt)
        {
            dt.signature = await GetSignatureAsync(dt.merchant_ref, dt.amount);
            dt.expired_time = DateTimeOffset.UtcNow.AddHours(24).ToUnixTimeSeconds();

            var json = JsonConvert.SerializeObject(dt);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("transaction/create", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<dynamic>(responseBody);
        }

        public async Task<string> GetSignatureAsync(string noReferensiMerchant, decimal nominal)
        {
            string data = _kodeMerchant + noReferensiMerchant + nominal.ToString("0");

            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_privateKey)))
            {
                byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        
    }

    public class TripayChannelResponse
    {
        public bool Success { get; set; }
        public List<Channel> Data { get; set; }
    }
}