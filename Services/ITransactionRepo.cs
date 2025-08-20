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
        Task<bool> Checkout(ReqCheckout data);
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
            string query;

            if (data.Id == 0) // INSERT baru
            {
                query = @"
                    INSERT INTO log_transaction 
                    (
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
                    RETURNING id;";
            }
            else // UPDATE existing
            {
                query = @"
                    UPDATE log_transaction
                    SET
                        kategori_layanan   = @KategoriLayanan,
                        jenis_layanan      = @JenisLayanan,
                        total_transaksi    = @TotalTransaksi,
                        status             = @Status,
                        status_deskripsi   = @StatusDeskripsi,
                        jenis_properti     = @JenisProperti::jsonb,
                        kontak_pelanggan   = @KontakPelanggan::jsonb,
                        alamat_pelanggan   = @AlamatPelanggan::jsonb,
                        kunjungan          = @Kunjungan::jsonb,
                        cart_item          = @CartItem::jsonb,
                        paket_member       = @PaketMember::jsonb,
                        keluhan_perbaikan  = @KeluhanPerbaikan::jsonb,
                        updated_at         = now()
                    WHERE id = @Id
                    RETURNING id;";
            }

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
        
        public async Task<bool> Checkout(ReqCheckout data)
        {
            var query = @"
                update log_transaction
                set 
                    status = 1,
                    status_deskripsi = 'Pesanan diterima, menunggu Pembayaran',
                    tripay_noreff = @reff,
                    checkout_date = now(),
                    tripay_reff = @TripayReq::jsonb
                where id = @Id
                RETURNING id;
            ";

            var param = new
            {
                Id = data.Id,
                reff = data.NoRefCheckout,
                TripayReq =  JsonConvert.SerializeObject(data.TripayReq)
            };
            var result = await _db.ExecuteScalarAsync<int>(query, param);
            return result > 0;
        }
    }
}