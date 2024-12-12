using System;

namespace API.DTOs
{
    public class ApplicantPreviousCompaniesDto
    {
        public string CompanyName { get; set; }
        public string Position { get; set; }
        public int StartYear { get; set; }
        public int? EndYear { get; set; }
        public string Description { get; set; }
        public int? UserCompanyId { get; set; }
    }
}
