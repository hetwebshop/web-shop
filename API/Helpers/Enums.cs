using System.Text.Json.Serialization;

namespace API.Helpers
{
    public enum RoleType
    {
        Admin,
        StoreModerator,
        TrackModerator,
        TrackAgent,
        TrackAdmin,
        StoreAdmin,
        StoreAgent,
        User
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
        Closed = 2
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
