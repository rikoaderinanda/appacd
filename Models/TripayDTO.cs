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

    public class SignatureDTO
    {
        public string KodeMerchant { get; set; }
        public string NoReferensiMerchant { get; set; }
        public decimal Nominal { get; set; }
        public string PrivateKey { get; set; }
    }

    public class TripayTransactionRequest
    {
        //[JsonPropertyName("method")]
        public string method { get; set; }

        //[JsonPropertyName("merchant_ref")]
        public string merchant_ref { get; set; }

        public int amount { get; set; }

        //[JsonPropertyName("customer_name")]
        public string customer_name { get; set; }

        //[JsonPropertyName("customer_email")]
        public string customer_email { get; set; }

        //[JsonPropertyName("customer_phone")]
        public string customer_phone { get; set; }

        //[JsonPropertyName("order_items")]
        public List<OrderItemDTO> order_items { get; set; }

        //[JsonPropertyName("callback_url")]
        public string callback_url { get; set; }
        
        //[JsonPropertyName("return_url")]
        public string return_url { get; set; }

        //[JsonPropertyName("expired_time")]
        public long expired_time { get; set; }

        public string signature { get; set; }
    }
    
    public class OrderItemDTO
    {
        public string sku { get; set; }
        public string name { get; set; }
        public int price { get; set; }
        public int quantity { get; set; }
    }

    public class TripayCallbackDto
    {
        public string reference { get; set; }
        public string merchant_ref { get; set; }
        public string payment_method { get; set; }
        public string payment_method_code { get; set; }
        public int total_amount { get; set; }
        public int fee_merchant { get; set; }
        public int fee_customer { get; set; }
        public int total_fee { get; set; }
        public int amount_received { get; set; }
        public int is_closed_payment { get; set; }
        public string status { get; set; }
        public long? paid_at { get; set; }
        public string note { get; set; }
    }

    public class NotifikasiRequest
    {
        public string Pesan { get; set; }
    }
}