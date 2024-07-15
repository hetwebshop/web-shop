using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API.Entities.JobPost
{
    public class JobCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<UserJobPost> UserJobPosts { get; set; }
        public ICollection<CompanyJobPost.CompanyJobPost> CompanyJobPosts { get; set; }
    }
}
