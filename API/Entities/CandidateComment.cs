using System;

namespace API.Entities
{
    public class CandidateComment
    {
        public int Id { get; set; }
        public int CandidateId { get; set; } // user Id of candidate
        public int CompanyUserId { get; set; } // user Id of company that set comment
        public string Comment { get; set; }
        public DateTime Date { get; set; }
        public int UserApplicationId { get; set; }
    }
}
