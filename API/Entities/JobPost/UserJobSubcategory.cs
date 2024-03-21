namespace API.Entities.JobPost
{
    public class UserJobSubcategory
    {
        public int UserJobPostId { get; set; }
        public UserJobPost UserJobPost { get; set; }

        public int JobCategoryId { get; set; }
        public JobCategory JobCategory { get; set; }
    }
}
