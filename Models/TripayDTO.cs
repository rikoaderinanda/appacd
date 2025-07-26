using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace appacd.Models
{
     public class Channel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public string FeeCustomer { get; set; }
        public string FeeMerchant { get; set; }
        public bool Active { get; set; }
    }
}