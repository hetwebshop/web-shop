using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API.Entities.JobPost
{
    public class JobCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        [JsonIgnore]
        public JobCategory ParentCategory { get; set; }
        public ICollection<JobCategory> Subcategories { get; set; }
        public ICollection<UserJobPost> UserJobPosts { get; set; }
    }
}
