using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using appacd.Models;
using Newtonsoft.Json;

namespace appacd.Services
{
    public interface IProductRepository
    {
        Task<IEnumerable<dynamic?>> GetProduct();
    }

    public class ProductRepository : IProductRepository
    {
        private readonly IDbConnection _db;

        public ProductRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IEnumerable<dynamic?>> GetProduct()
        {
            var sql = @"
                SELECT * FROM produk ORDER BY id asc
            ";
            return await _db.QueryAsync<dynamic>(sql);
        }

    }

    

    

}