using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using appacd.Models;
using Newtonsoft.Json;

namespace appacd.Services
{
    public interface IPerangkatRepository
    {
        Task<IEnumerable<dynamic>> GetPhotoAsync(string Id);
        Task<dynamic> CheckQrCodeUnit(string decodedText);
    }

    public class PerangkatRepository : IPerangkatRepository
    {
        private readonly IDbConnection _db;

        public PerangkatRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IEnumerable<dynamic>> GetPhotoAsync(string Id)
        {
            var finalResult = new List<dynamic>();
            string sql = @"
                SELECT to_jsonb(oi)
                FROM photo_perangkat oi
                WHERE oi.id = @id;
            ";
            var result = await _db.QueryAsync<string>(sql,new { id = int.Parse(Id)});
            foreach (var jsonString in result)
            {
                try
                {
                    dynamic? obj = JsonConvert.DeserializeObject<dynamic>(jsonString);
                    if (obj is not null)
                    {
                        finalResult.Add(obj);
                    }
                }
                catch (JsonException ex)
                {
                    Console.WriteLine("Gagal parsing JSON: " + ex.Message);
                }
            }
            return finalResult;
        }

        public async Task<dynamic?> CheckQrCodeUnit(string decodedText)
        {
            string sql = @"
                SELECT to_jsonb(oi)
                FROM perangkat_pelanggan oi
                WHERE oi.qr_code = @code;
            ";

            var result = await _db.QueryFirstOrDefaultAsync<string>(sql, new { code = decodedText });

            if (string.IsNullOrWhiteSpace(result))
                return null;

            dynamic obj = JsonConvert.DeserializeObject<dynamic>(result);
            return obj;
        }
    }
}