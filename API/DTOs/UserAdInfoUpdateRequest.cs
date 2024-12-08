namespace API.DTOs
{
    public class UserAdInfoUpdateRequest
    {
        public string AdTitle { get; set; }
        public string Position { get; set; }
        public int JobTypeId { get; set; }
        public string Biography { get; set; }
        public int JobCategoryId { get; set; }
        public int UserAdId { get; set; }
    }
}
