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
        public string Name { get; set; }
        public int? PhotoId { get; set; }
        public Photo Photo { get; set; }
        public int? AddressId { get; set; }
        public UserAddress Address { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<UserJobPost> UserJobPosts { get; set; }
    }
}
