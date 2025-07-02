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
}