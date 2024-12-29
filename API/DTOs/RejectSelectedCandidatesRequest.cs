using System.Collections.Generic;

namespace API.DTOs
{
    public class RejectSelectedCandidatesRequest
    {
        public List<int> Candidates { get; set; }
        public string Feedback { get; set; }
    }
}
