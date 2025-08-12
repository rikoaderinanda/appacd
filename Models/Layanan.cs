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

    public class AlamatPelanggan
    {
        public int Id { get; set; }
        public string Judul { get; set; }
        public string Alamat { get; set; }

        public string ProvinsiCode { get; set; }
        public string ProvinsiNama { get; set; }

        public string KotaCode { get; set; }
        public string KotaNama { get; set; }

        public string KecamatanCode { get; set; }
        public string KecamatanNama { get; set; }

        public string KelurahanCode { get; set; }
        public string KelurahanNama { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int IdUser { get; set; }
    }
}