namespace API.DTOs
{
    public class PaymentSessionResponseDto
    {
        public string Status { get; set; }
        public string ClientSecret { get; set; }
        public string Error { get; set; }
    }
}
