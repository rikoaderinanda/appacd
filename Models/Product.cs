using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace appacd.Models
{
    public class Produk
    {
        public string Id { get; set; }
        public string NamaLayanan { get; set; }
        public string ImgLayanan { get; set; }
        public decimal Harga { get; set; }
        public double Rating { get; set; }
        public int JumlahPembeli { get; set; }
    }
}
