using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using appacd.Models;
using Dapper;

namespace appacd.Services
{
    public interface ILayananRepository
    {
        Task<IEnumerable<dynamic>> GetAllAsync();
    }
    public class LayananRepository : ILayananRepository
    {
        private readonly IDbConnection _db;

        public LayananRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IEnumerable<dynamic>> GetAllAsync()
        {
            var sql = "SELECT * FROM listlayanan ORDER BY created_at DESC";
            return await _db.QueryAsync<Layanan>(sql);
        }
    }
}