using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace appacd.Models
{ 
    public class LogTransaction
    {
        public long Id { get; set; } // bigserial → long
        public int CreateByIdUser { get; set; }
        public string KategoriLayanan { get; set; } = string.Empty;
        public string JenisLayanan { get; set; } = string.Empty;
        public float TotalTransaksi { get; set; }
        public int Status { get; set; }
        public string? StatusDeskripsi { get; set; }

        public object? JenisProperti { get; set; } //Json data
        public object? kontakPelanggan { get; set; } //Json data
        public object? alamatPelanggan { get; set; } //Json data
        public object? kunjungan { get; set; } //Json data
        public object? cartItem { get; set; } //json data
        public object? paketMember { get; set; } //json data

    }
}