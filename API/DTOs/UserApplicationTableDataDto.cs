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
    }
}
