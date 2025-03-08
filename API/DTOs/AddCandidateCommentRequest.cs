namespace API.DTOs
{
    public class AddCandidateCommentRequest
    {
        public string Comment { get; set; }
        public int CandidateId { get; set; }
        public int UserApplicationId { get; set; }
        public int CompanyJobPostId { get; set; }
    }
}
