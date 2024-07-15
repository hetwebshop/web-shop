using System.Text.Json.Serialization;

namespace API.Helpers
{
    public enum RoleType
    {
        Admin,
        User,
        Company
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Gender
    {
        Male,
        Female,
        Other
    }

    public enum JobPostStatus
    {
        Active = 1,
        Closed = 2,
        Deleted = 3,
    }

    public enum LocationType
    {
        Area,
        City,
        State,
        Country
    }

    public enum AdvertisementTypeEnum
    {
        JobAd = 1,
        Service
    }
}
