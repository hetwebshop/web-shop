using System.Collections.Generic;

namespace API.Entities.JobPost
{
    public class AdvertisementType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<UserJobPost> UserJobPosts { get; set; }
    }
}
