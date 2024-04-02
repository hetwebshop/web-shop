namespace API.DTOs
{
    public interface FilterCriteria
    {
        string SearchKeyword { get; set; }
        int CityId { get; set; }
        int JobCategoryId { get; set; }
        int JobTypeId { get; set; }
        int AdvertisementTypeId { get; set; }
    }

}
