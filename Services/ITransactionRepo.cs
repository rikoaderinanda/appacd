using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using appacd.Models;
using Dapper;
using Newtonsoft.Json;

namespace appacd.Services
{
    public interface ITransactionRepo
    {
        Task<int> SimpanAsync(LogTransaction data);
        Task<IEnumerable<dynamic>> GetCart_ByUserId(string id);
        Task<bool> DeleteCart(string _id);
        Task<IEnumerable<dynamic>> GetDetail_ById(string id);
    }
    public class TransactionRepo : ITransactionRepo
    {
        private readonly IDbConnection _db;

        public TransactionRepo(IDbConnection db)
        {
            _db = db;
        }

        public async Task<int> SimpanAsync(LogTransaction data)
        {
            var query = @"
                INSERT INTO log_transaction 
                (
                    id,
                    create_by_id_user,
                    kategori_layanan,
                    jenis_layanan,
                    total_transaksi,
                    status,
                    status_deskripsi,
                    jenis_properti,
                    kontak_pelanggan,
                    alamat_pelanggan,
                    kunjungan,
                    cart_item,
                    paket_member,
                    keluhan_perbaikan
                ) 
                VALUES 
                (
                    NULLIF(@Id, 0), -- kalau 0 => NULL (biar auto generate)
                    @CreateByIdUser,
                    @KategoriLayanan,
                    @JenisLayanan,
                    @TotalTransaksi,
                    @Status,
                    @StatusDeskripsi,
                    @JenisProperti::jsonb,
                    @KontakPelanggan::jsonb,
                    @AlamatPelanggan::jsonb,
                    @Kunjungan::jsonb,
                    @CartItem::jsonb,
                    @PaketMember::jsonb,
                    @KeluhanPerbaikan::jsonb
                )
                ON CONFLICT (id) DO UPDATE SET
                    kategori_layanan   = EXCLUDED.kategori_layanan,
                    jenis_layanan      = EXCLUDED.jenis_layanan,
                    total_transaksi    = EXCLUDED.total_transaksi,
                    status             = EXCLUDED.status,
                    status_deskripsi   = EXCLUDED.status_deskripsi,
                    jenis_properti     = EXCLUDED.jenis_properti,
                    kontak_pelanggan   = EXCLUDED.kontak_pelanggan,
                    alamat_pelanggan   = EXCLUDED.alamat_pelanggan,
                    kunjungan          = EXCLUDED.kunjungan,
                    cart_item          = EXCLUDED.cart_item,
                    paket_member       = EXCLUDED.paket_member,
                    keluhan_perbaikan  = EXCLUDED.keluhan_perbaikan
                RETURNING id;
            ";

            var param = new
            {
                Id = data.Id,
                CreateByIdUser = data.CreateByIdUser,
                KategoriLayanan = data.KategoriLayanan,
                JenisLayanan = data.JenisLayanan,
                TotalTransaksi = data.TotalTransaksi,
                Status = data.Status,
                StatusDeskripsi = data.StatusDeskripsi,
                JenisProperti = JsonConvert.SerializeObject(data.JenisProperti),
                KontakPelanggan = JsonConvert.SerializeObject(data.kontakPelanggan),
                AlamatPelanggan = JsonConvert.SerializeObject(data.alamatPelanggan),
                Kunjungan = JsonConvert.SerializeObject(data.kunjungan),
                CartItem = JsonConvert.SerializeObject(data.cartItem),
                PaketMember = JsonConvert.SerializeObject(data.paketMember),
                KeluhanPerbaikan = JsonConvert.SerializeObject(data.keluhanPerbaikan)
            };

            int Id = await _db.ExecuteScalarAsync<int>(query, param);
            return Id;
        }


        public async Task<IEnumerable<dynamic>> GetDetail_ById(string id)
        {
            var sql = @"
                SELECT
                    lt.id,
                    lt.kategori_layanan,
                    lt.jenis_layanan,
                    lt.total_transaksi,
                    (cart_item::jsonb)->'Reguler' AS cart_items,
                    lt.jenis_properti::jsonb AS properti,    
                    lt.kontak_pelanggan::jsonb as kontak,
                    lt.alamat_pelanggan::jsonb as alamat_pelanggan,
                    lt.kunjungan::jsonb as kunjungan,
                    paket_member::jsonb AS paket,
                    create_by_id_user
                FROM log_transaction lt 
                WHERE lt.id = @Id::bigint;
            ";

            var param = new { Id = id };
            var result = await _db.QueryAsync<dynamic>(sql, param);

            return JsonColumnParser.ParseJsonColumns(result);
        }

        public async Task<IEnumerable<dynamic>> GetCart_ByUserId(string id)
        {
            var sql = @"
                SELECT
                    lt.id,
                    lt.kategori_layanan,
                    lt.jenis_layanan,
                    lt.total_transaksi,
                    (cart_item::jsonb)->'Reguler' AS cart_items,
                    lt.jenis_properti::jsonb AS properti,
                    paket_member::jsonb AS paket
                FROM log_transaction lt 
                WHERE lt.create_by_id_user = @Id::bigint 
                AND status = 0
                ORDER BY id DESC;
            ";

            var param = new { Id = id };
            var result = await _db.QueryAsync<dynamic>(sql, param);

            return JsonColumnParser.ParseJsonColumns(result);
        }

        public async Task<bool> DeleteCart(string _id)
        {
            var query = "DELETE FROM log_transaction WHERE id = @Id::bigint;";
            var result = await _db.ExecuteAsync(query, new { Id = _id });
            return result > 0;
        }
        
    }
}