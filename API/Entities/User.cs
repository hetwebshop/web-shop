using API.Entities.JobPost;
using API.Helpers;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace API.Entities
{
    public class User : IdentityUser<int>
    {
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public DateTime LastActive { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? PhotoId { get; set; }
        public Photo Photo { get; set; }
        public int CityId { get; set; }
        public City City { get; set; }
        public int? JobTypeId { get; set; }
        public JobType JobType { get; set; }
        public int? JobCategoryId { get; set; }
        public JobCategory JobCategory { get; set; }
        public string Position { get; set; }
        public string Biography { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<UserJobPost> UserJobPosts { get; set; }
        public ICollection<UserEducation> UserEducations { get; set; }
    }
}
