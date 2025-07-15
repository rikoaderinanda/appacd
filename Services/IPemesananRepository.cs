using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using appacd.Models;
using Dapper;
using Newtonsoft.Json;

namespace appacd.Services
{
    public interface IPemesananRepository
    {
        Task<int> SimpanAsync(PemesananDto dto);
        Task<int> HapusPemesananAsync(int id);
        Task<IEnumerable<dynamic>> GetPemesanan();
        Task<IEnumerable<dynamic>> GetPemesananById(string id);
        Task<IEnumerable<dynamic>> GetPemesanan_Keranjang(string id);

    }
    public class PemesananRepository : IPemesananRepository
    {
        private readonly IDbConnection _db;

        public PemesananRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IEnumerable<dynamic>> GetPemesanan()
        {
            var finalResult = new List<dynamic>();
            const string sql = "select order_json::text from get_pemesanan_full()";
            var result = await _db.QueryAsync<string>(sql);
            Console.WriteLine(result.First().GetType().Name);
            foreach (var jsonString in result)
            {
                Console.WriteLine("Raw JSON: " + jsonString);
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

        public async Task<IEnumerable<dynamic>> GetPemesananById(string id)
        {
            var finalResult = new List<dynamic>();
            string qry = "select order_json::text from get_pemesanan_byId(" + id + ")";
            var result = await _db.QueryAsync<string>(qry);
            Console.WriteLine(result.First().GetType().Name);
            foreach (var jsonString in result)
            {
                Console.WriteLine("Raw JSON: " + jsonString);
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


        public async Task<IEnumerable<dynamic>> GetPemesanan_Keranjang(string id)
        {
            var sql = "SELECT * FROM pemesanan_keranjang WHERE pemesanan_id = @id";
            return await _db.QueryAsync<dynamic>(sql, new { id });
        }

        public async Task<int> SimpanAsync(PemesananDto data)
        {

            if (data.Id != 0)
            {
                var query = "SELECT sp_update_pemesanan(" +
                                "@Id,@id_layanan," +
                                "@nama, @whatsapp, @catatan, @lokasi, @alamat, @total, @tanggalKunjungan::date," +
                                "@jamKunjungan::time," +
                                "@keranjang_json::jsonb," +
                                "@jenisproperti_json::jsonb," +
                                "@keluhan_json::jsonb)";

                // Console.WriteLine($"nama: {data.Customer.nama}");
                // Console.WriteLine($"whatsapp: {data.Customer.whatsapp}");
                // Console.WriteLine($"catatan: {data.Customer.catatan}");
                // Console.WriteLine($"lokasi: {data.Customer.lokasi}");
                // Console.WriteLine($"alamat: {data.Customer.alamat}");

                var param = new
                {
                    Id = data.Id,
                    id_layanan = data.id_layanan,
                    nama = data.Customer.nama,
                    whatsapp = data.Customer.whatsapp,
                    catatan = data.Customer.catatan,
                    lokasi = data.Customer.lokasi,
                    alamat = data.Customer.alamat,
                    total = data.Total,
                    tanggalKunjungan = DateTime.Parse(data.TanggalKunjungan),
                    jamKunjungan = TimeSpan.Parse(data.JamKunjungan),
                    keranjang_json = JsonConvert.SerializeObject(data.Keranjang),
                    jenisproperti_json = JsonConvert.SerializeObject(data.JenisProperti),
                    keluhan_json = JsonConvert.SerializeObject(data.Keluhan)
                };
                int pemesananId = await _db.ExecuteScalarAsync<int>(query, param);
                return pemesananId;
            }
            else
            {
                var query = "SELECT sp_simpan_pemesanan(" +
                "@id_layanan,@nama, @whatsapp, @catatan, @lokasi, @alamat, @total, @tanggalKunjungan::date," +
                "@jamKunjungan::time," +
                "@keranjang_json::jsonb," +
                "@jenisproperti_json::jsonb," +
                "@keluhan_json::jsonb)";
                // Console.WriteLine($"nama: {data.Customer.nama}");
                // Console.WriteLine($"whatsapp: {data.Customer.whatsapp}");
                // Console.WriteLine($"catatan: {data.Customer.catatan}");
                // Console.WriteLine($"lokasi: {data.Customer.lokasi}");
                // Console.WriteLine($"alamat: {data.Customer.alamat}");
                var param = new
                {
                    id_layanan = data.id_layanan,
                    nama = data.Customer.nama,
                    whatsapp = data.Customer.whatsapp,
                    catatan = data.Customer.catatan,
                    lokasi = data.Customer.lokasi,
                    alamat = data.Customer.alamat,
                    total = data.Total,
                    tanggalKunjungan = DateTime.Parse(data.TanggalKunjungan),
                    jamKunjungan = TimeSpan.Parse(data.JamKunjungan),
                    keranjang_json = JsonConvert.SerializeObject(data.Keranjang),
                    jenisproperti_json = JsonConvert.SerializeObject(data.JenisProperti),
                    keluhan_json = JsonConvert.SerializeObject(data.Keluhan)
                };

                int pemesananId = await _db.ExecuteScalarAsync<int>(query, param);
                return pemesananId;
            }
        }

        public async Task<int> HapusPemesananAsync(int id)
        {
            var query = "SELECT sp_hapus_pemesanan(@Id)";
            var param = new
            {
                Id = id
            };

            int resId = await _db.ExecuteScalarAsync<int>(query, param);
            return resId;
        }

    }
}