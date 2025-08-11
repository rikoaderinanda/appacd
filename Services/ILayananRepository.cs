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
        Task<IEnumerable<dynamic>> GetLayananById(string Id);
        Task<IEnumerable<dynamic>> InfoLayananAsync(string Id);
        Task<IEnumerable<dynamic>> KeluhanMasalahAsync(string Id);
        Task<IEnumerable<dynamic>> JasaLayananAsync(string Id);
        Task<IEnumerable<dynamic>> JenisPropertiAsync(string Id);
        Task<IEnumerable<dynamic>> BannerLayananAsync(string Id);
        Task<IEnumerable<dynamic>> JasaLayananDetailAsync(string Id);
        Task<IEnumerable<dynamic>> LanggananJasaAsync(string Id);
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
            var sql = "SELECT * FROM listlayanan ORDER BY id asc";
            return await _db.QueryAsync<dynamic>(sql);
        }

        public async Task<IEnumerable<dynamic>> GetLayananById(string Id)
        {
            var sql = "SELECT * FROM listlayanan where id="+Id+" ORDER BY created_at DESC";
            return await _db.QueryAsync<dynamic>(sql);
        }

        public async Task<IEnumerable<dynamic>> InfoLayananAsync(string Id)
        {
            var sql = "SELECT * FROM infolayanan where id_layanan = "+Id+" ORDER BY urutan asc";
            return await _db.QueryAsync<dynamic>(sql);
        }

        public async Task<IEnumerable<dynamic>> KeluhanMasalahAsync(string Id)
        {
            var sql = "SELECT * FROM keluhanmasalah where idlayanan = "+Id+" ORDER BY urutan asc";
            return await _db.QueryAsync<dynamic>(sql);
        }
        public async Task<IEnumerable<dynamic>> JasaLayananAsync(string Id)
        {
            var sql = "SELECT * FROM jasalayanan where id_layanan ="+Id+ " ORDER BY urutan asc";
            return await _db.QueryAsync<dynamic>(sql);
        }

        public async Task<IEnumerable<dynamic>> JenisPropertiAsync(string Id)
        {
            var sql = "SELECT * FROM jenis_properti where id_layanan = "+Id+" ORDER BY urutan asc";
            return await _db.QueryAsync<dynamic>(sql);
        }

        public async Task<IEnumerable<dynamic>> BannerLayananAsync(string Id)
        {
            var sql = "SELECT * FROM banner_layanan where id_layanan = " + Id + " ORDER BY id asc";
            return await _db.QueryAsync<dynamic>(sql);
        }

        public async Task<IEnumerable<dynamic>> JasaLayananDetailAsync(string Id)
        {
            var sql = "SELECT * FROM jasa_layanan_detail where id_jasalayanan = " + Id + " ORDER BY id asc";
            return await _db.QueryAsync<dynamic>(sql);
        }

        public async Task<IEnumerable<dynamic>> LanggananJasaAsync(string Id)
        {
            var sql = "SELECT * FROM langganan where id_layanan = " + Id + " ORDER BY id asc";
            return await _db.QueryAsync<dynamic>(sql);
        }


    }
}