using API.Entities.JobPost;

namespace API.Entities
{
    public class CompanyJobCategoryInterests
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } 
        public int JobCategoryId { get; set; }
        public JobCategory JobCategory { get; set; }
    }
}
