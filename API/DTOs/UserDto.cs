namespace API.DTOs
{
    public class UserDto
    {
        public string UserName { get; set; }
        public string Token { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int CityId { get; set; }
        public string PhotoUrl { get; set; }
        public string Email { get; set; }
    }

    public class UserInfoDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int CityId { get; set; }
        public string PhotoUrl { get; set; }
        public bool Exist { get; set; }
    }
}
