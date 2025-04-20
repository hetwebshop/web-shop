using API.Entities.Applications;
using API.Helpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace API.DTOs
{
    public class UserApplicationTableDataDto
    {
        public int CompanyJobPostId { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public string CompanyCity { get; set; }
        public ApplicationStatus ApplicationStatusId { get; set; }
        public string JobPosition { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Feedback { get; set; }
        public DateTime? MeetingDateTime { get; set; }
        public int Id { get; set; }
        public bool IsCompanyAdExpired { get; set; }
        public DateTime CompanyAdEndDate { get; set; }
        public int? ConversationId { get; set; }
        public string OnlineMeetingLink { get; set; }
        public string MeetingPlace { get; set; }
    }
}
