using System;

namespace API.DTOs
{
    public class MonriCallbackRequest
    {
        public int id { get; set; }
        public string acquirer { get; set; }
        public string order_number { get; set; }
        public int amount { get; set; }
        public string currency { get; set; }
        public string ch_full_name { get; set; }
        public int outgoing_amount { get; set; }
        public string outgoing_currency { get; set; }
        public string approval_code { get; set; }
        public string response_code { get; set; }
        public string response_message { get; set; }
        public string reference_number { get; set; }
        public string systan { get; set; }
        public string eci { get; set; }
        public string xid { get; set; }
        public string acsv { get; set; }
        public string cc_type { get; set; }
        public string status { get; set; }
        public DateTime created_at { get; set; }
        public string transaction_type { get; set; }
        public string enrollment { get; set; }
        public string authentication { get; set; }
        public string pan_token { get; set; }
        public string masked_pan { get; set; }
        public string issuer { get; set; }
        public int? number_of_installments { get; set; }
        public string custom_params { get; set; }
    }
}
