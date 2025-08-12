using System;
using System.ComponentModel.DataAnnotations;

namespace appacd.Models
{
    public class Layanan
    {
        public int Id { get; set; }

        public string Nama_Layanan { get; set; } = string.Empty;

        public string? Deskripsi { get; set; }

        public decimal Harga { get; set; }

        public bool Aktif { get; set; }

        public DateTime Created_At { get; set; }
    }

    public class Kontak
    {
        public int Id { get; set; }             // serial4 (auto-increment)
        public DateTime CreateAt { get; set; }  // timestamp
        public long UserId { get; set; }        // int8 (bigint)
        public string NamaKontak { get; set; }  // varchar
        public string NomorKontak { get; set; } // varchar
    }
}