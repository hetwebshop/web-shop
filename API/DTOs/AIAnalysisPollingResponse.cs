namespace API.DTOs
{
    public class AIAnalysisPollingResponse
    {
        public double? Progress { get; set; }
        public bool HasError { get; set; }
        public string Error { get; set; }
    }
}
