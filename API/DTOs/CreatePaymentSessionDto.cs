namespace API.DTOs
{
    public class CreatePaymentSessionDto
    {
        public int amount { get; set; } // amount is in minor units, ie. 10.24 USD is sent as 1024
        public string order_number { get; set; }
        public string currency { get; set; }
        public string transaction_type { get; set; }
        public string order_info { get; set; }
        public string scenario { get; set; }
        public string[] supported_payment_methods { get; set; }
    }

}
