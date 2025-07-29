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
        Task<int> PembayaranSukses(int id);
        Task<int> VerifikasiPembayaran(int id);

        Task<int> HapusTrackingById(int id);
        Task<IEnumerable<dynamic>> GetTrackingById(int Id);
        Task<IEnumerable<dynamic>> GetTracking(string id);
        Task<int> CheckoutAsync(int Id,string NoRefCheckout);

        Task<int> SimpanAsync(PemesananDto dto);
        Task<int> HapusPemesananAsync(int id);
        Task<IEnumerable<dynamic>> GetPemesanan(string id);
        Task<IEnumerable<dynamic>> GetPemesananById(string id);
        Task<IEnumerable<dynamic>> GetPemesanan_Keranjang(string id);

        Task<IEnumerable<dynamic>> GetStepStatus();

        Task<dynamic> GetInvoiceByReference(string tripayReference);
        Task<bool> UpdateInvoiceStatusAsync(string Id, string Status);

    }
    public class PemesananRepository : IPemesananRepository
    {
        private readonly IDbConnection _db;

        public PemesananRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IEnumerable<dynamic>> GetPemesanan(string id)
        {
            var finalResult = new List<dynamic>();
            string sql = "select order_json::text from get_pemesanan_full(" + id + ")";
            var result = await _db.QueryAsync<string>(sql);
            // Console.WriteLine(result.First().GetType().Name);
            foreach (var jsonString in result)
            {
                // Console.WriteLine("Raw JSON: " + jsonString);
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
                // Console.WriteLine("Raw JSON: " + jsonString);
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
                var query = @"SELECT sp_update_pemesanan(
                                @Id,@id_layanan,
                                @nama, @whatsapp, @catatan, @lokasi, @alamat, @total, @tanggalKunjungan::date,
                                @jamKunjungan::time,
                                @keranjang_json::jsonb,
                                @jenisproperti_json::jsonb,
                                @keluhan_json::jsonb,
                                @user_id)
                            ";

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
                    keluhan_json = JsonConvert.SerializeObject(data.Keluhan),
                    user_id = data.user_id

                };
                int pemesananId = await _db.ExecuteScalarAsync<int>(query, param);
                return pemesananId;
            }
            else
            {
                var query = 
                @"SELECT sp_simpan_pemesanan
                (
                    @id_layanan,@nama, @whatsapp, @catatan, @lokasi, @alamat, @total, @tanggalKunjungan::date,
                    @jamKunjungan::time,
                    @keranjang_json::jsonb,
                    @jenisproperti_json::jsonb,
                    @keluhan_json::jsonb,
                    @user_id
                )";
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
                    keluhan_json = JsonConvert.SerializeObject(data.Keluhan),
                    user_id = data.user_id
                };

                int pemesananId = await _db.ExecuteScalarAsync<int>(query, param);
                return pemesananId;
            }
        }

        public async Task<int> CheckoutAsync(int Id,string _NoRefCheckout)
        {
            var query = @"
                            select sp_checkout_pemesanan(@Id,@NoRefCheckout);
                        ";

            var param = new
            {
                Id = Id,
                NoRefCheckout = _NoRefCheckout
            };

            int pemesananId = await _db.ExecuteScalarAsync<int>(query, param);

            return pemesananId;
        }

        public async Task<IEnumerable<dynamic>> GetTracking(string id)
        {
            if (!int.TryParse(id, out var userId))
                throw new ArgumentException("Invalid user ID");

            var sql = @"
                SELECT a.*, b.status_tracking, b.status_tracking_color 
                FROM pemesanan a 
                LEFT JOIN list_status_order b ON b.id = a.status_order
                WHERE a.status_order > 0
                AND a.order_by_user_id = @id
                ORDER BY a.status_order ASC
            ";
            return await _db.QueryAsync<dynamic>(sql, new { id = userId });
        }

        public async Task<IEnumerable<dynamic>> GetTrackingById(int Id)
        {
            var sql = @"
                select a.*,b.status_tracking,b.status_tracking_color from pemesanan a 
                left join list_status_order b on b.id = a.status_order
                where a.status_order > 0 and a.id = @Id
            ";
            var param = new
            {
                Id = Id
            };

            return await _db.QueryAsync<dynamic>(sql, param);
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

        public async Task<int> HapusTrackingById(int id)
        {
            var query = "SELECT sp_hapus_tracking_byid(@Id)";
            var param = new
            {
                Id = id
            };

            int resId = await _db.ExecuteScalarAsync<int>(query, param);
            return resId;
        }

        public async Task<int> PembayaranSukses(int id)
        {
            var query = "SELECT sp_PembayaranSukses(@Id)";
            var param = new
            {
                Id = id
            };

            int resId = await _db.ExecuteScalarAsync<int>(query, param);
            return resId;
        }

        public async Task<int> VerifikasiPembayaran(int id)
        {
            var query = "SELECT sp_verifikasi_pembayaran(@Id)";
            var param = new
            {
                Id = id
            };

            int resId = await _db.ExecuteScalarAsync<int>(query, param);
            return resId;
        }

        public async Task<IEnumerable<dynamic>> GetStepStatus()
        {
            var sql = "SELECT * FROM list_status_order order by id asc";
            return await _db.QueryAsync<dynamic>(sql);
        }

        public async Task<dynamic> GetInvoiceByReference(string _tripayReference)
        {
            var sql = @"
                select *
                from pemesanan 
                where no_ref_checkout = @reference
            ";
            return await _db.QueryAsync<dynamic>(sql, new { reference = _tripayReference });
        }
        
        public async Task<bool> UpdateInvoiceStatusAsync(string id, string status)
        {
            if (!int.TryParse(id, out var parsedId))
                throw new ArgumentException("Invalid id", nameof(id));

            var sql = @"
                UPDATE pemesanan
                SET tripay_status = @status
                WHERE id = @id
            ";

            var affectedRows = await _db.ExecuteAsync(sql, new { id = parsedId, status });
            return affectedRows > 0;
        }
    }
}