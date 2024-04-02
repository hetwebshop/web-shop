using API.Entities.JobPost;
using System.Collections.Generic;

namespace API.Entities
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PostalCode { get; set; }

        public int CountryId { get; set; }
        public Country Country { get; set; }
        //public ICollection<Municipality> Municipalities { get; set; }
        //public int? CantonId { get; set; }
        //public Canton Canton { get; set; }
        //public ICollection<UserJobPost> UserJobPosts { get; set; }
    }
}
