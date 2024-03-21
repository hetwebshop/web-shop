namespace API.Entities
{
    public class UserAddress
    {
        public int Id { get; set; }
        public string StreetName { get; set; }
        public string StreetNumber { get; set; }

        public User User { get; set; }
        public int CityId { get; set; }
        public City City { get; set; }

        public UserAddress(string streetName, string streetNumber, int cityId)
        {
            StreetName = streetName;
            StreetNumber = streetNumber;
            CityId = cityId;
        }
        public UserAddress() { }

        //public int? MunicipalityId { get; set; }
        //public Municipality Municipality { get; set; }
    }
}
