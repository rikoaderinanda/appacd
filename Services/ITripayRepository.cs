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
        Task<dynamic> GetDetailTransactionAsync(string reference);
        Task<string> GetSignatureCallbackAsync(string data);
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

        public async Task<dynamic> GetDetailTransactionAsync(string reference)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(reference))
                    throw new ArgumentException("Reference tidak boleh kosong.");

                var response = await _httpClient.GetAsync("transaction/detail?reference=" + reference);

                // Jika status code bukan 2xx
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Tripay API error ({(int)response.StatusCode}): {response.ReasonPhrase}\n{errorContent}");
                }

                var responseBody = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(responseBody))
                    throw new Exception("Response dari Tripay kosong.");

                return JsonConvert.DeserializeObject<dynamic>(responseBody);
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"[HTTP Error] {httpEx.Message}");
                throw; // Lempar lagi ke caller atau bisa dikembalikan nilai default
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"[JSON Parsing Error] {jsonEx.Message}");
                throw new Exception("Gagal memproses response JSON dari Tripay.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[General Error] {ex.Message}");
                throw new Exception("Terjadi kesalahan saat mengambil detail transaksi.");
            }
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

        public async Task<string> GetSignatureCallbackAsync(string data)
        {
            
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