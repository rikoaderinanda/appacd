using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace appacd.Models
{
    public class GeneralRequest
    {
        public int Id { get; set; }
    }
    public class HapusPemesananRequest
    {
        public int Id { get; set; }
    }

    public class PemesananDto
    {
        public List<KeranjangItem> Keranjang { get; set; }
        public List<JenisPropertiItem> JenisProperti { get; set; }
        public List<KeluhanItem> Keluhan { get; set; }
        public string TanggalKunjungan { get; set; }
        public string JamKunjungan { get; set; }
        public int Total { get; set; }
        public CustomerInfo Customer { get; set; }
        public int Id { get; set; }
        public int id_layanan { get; set; }
        public int user_id{get;set;}
    }

    public class UserInfo
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string NamaLengkap { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class KeranjangItem
    {
        public int id { get; set; }
        public string nama { get; set; }
        public int jumlah { get; set; }
        public int harga { get; set; }
        public int total { get; set; }
    }
    public class JenisPropertiItem
    {
        public string id { get; set; }
        public string nama_jenis { get; set; }
        public int harga { get; set; }
    }
    public class KeluhanItem
    {
        public string id { get; set; }
        public string val { get; set; }
    }
    public class CustomerInfo
    {
        [JsonPropertyName("nama")]
        public string nama { get; set; }

        [JsonPropertyName("whatsapp")]
        public string whatsapp { get; set; }

        [JsonPropertyName("catatan")]
        public string catatan { get; set; }

        [JsonPropertyName("lokasi")]
        public string lokasi { get; set; }

        [JsonPropertyName("alamat")]
        public string alamat { get; set; }
    }
}