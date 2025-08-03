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

    }

    

    

}