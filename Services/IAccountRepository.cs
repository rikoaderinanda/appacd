using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using appacd.Models;
using Newtonsoft.Json;

namespace appacd.Services
{
    public interface IAccountRepository
    {
        Task<dynamic?> CheckUser(string email);
        Task<long?> CreateNewUserGoogle(GoogleNewUser user);
        Task<IEnumerable<dynamic?>> GetListKontakAsync(string IdUser);
        Task<bool> SimpanKontakAsync(Kontak data);
        Task<bool> DeleteKontak(string _id);
        Task<IEnumerable<dynamic?>> GetAlamatAsync(string IdUser);
    }

    public class AccountRepository : IAccountRepository
    {
        private readonly IDbConnection _db;

        public AccountRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<dynamic?> CheckUser(string _email)
        {
            string sql = @"
                SELECT to_jsonb(oi)
                FROM pelanggan_data oi
                WHERE oi.email = @email;
            ";

            var result = await _db.QueryFirstOrDefaultAsync<string>(sql, new { email = _email });

            if (string.IsNullOrWhiteSpace(result))
                return null;

            dynamic obj = JsonConvert.DeserializeObject<dynamic>(result);
            return obj;
        }

        public async Task<long?> CreateNewUserGoogle(GoogleNewUser user)
        {
            string sql = @"
                INSERT INTO pelanggan_data
                (
                    user_id_google,
                    username,
                    email,
                    photo
                )
                VALUES (
                    @user_id_google,
                    @username,
                    @email,
                    @photo
                )
                RETURNING id;
            ";

            var param = new {
                user_id_google = user.UserIdGoogle,
                username = user.Name,
                email = user.Email,      
                photo = user.Picture     
            };

            var id = await _db.QueryFirstOrDefaultAsync<long?>(sql, param);
            return id;
        }

        public async Task<IEnumerable<dynamic?>> GetListKontakAsync(string IdUser)
        {
            var sql = "SELECT * FROM kontak_pelanggan where user_id = @Id::bigint ORDER BY id asc";
            var param = new
            {
                Id = IdUser
            };

            return await _db.QueryAsync<dynamic>(sql, param);
        }

        public async Task<bool> SimpanKontakAsync(Kontak data)
        {
            try
            {
                var sql = @"
                    INSERT INTO public.kontak_pelanggan (user_id,nama_kontak,nomor_kontak)
                	VALUES (@_user_id,@_nama_kontak,@_nomor_kontak)
                    ON CONFLICT (user_id, nama_kontak, nomor_kontak) DO NOTHING;
                ";

                var param = new
                {
                    _user_id = data.UserId,
                    _nama_kontak = data.NamaKontak,
                    _nomor_kontak = data.NomorKontak
                };

                var affectedRows = await _db.ExecuteAsync(sql, param);
                return affectedRows > 0;
            }
            catch (Exception ex)
            {
                // Log exception jika kamu pakai logger
                // _logger.LogError(ex, "Error updating invoice status for ID: {Id}", id);
                return false; // bisa juga throw lagi kalau ingin controller yang menangani
            }
        }

        public async Task<bool> DeleteKontak(string _id)
        {
            var query = "DELETE FROM kontak_pelanggan WHERE id = @Id::bigint;";
            var result = await _db.ExecuteAsync(query, new { Id = _id });
            return result > 0;
        }

        public async Task<IEnumerable<dynamic?>> GetAlamatAsync(string IdUser)
        {
            var sql = "SELECT * FROM alamat_pelanggan where id_user = @Id::bigint ORDER BY id asc";
            var param = new
            {
                Id = IdUser
            };

            return await _db.QueryAsync<dynamic>(sql, param);
        }

    }

    

    

}