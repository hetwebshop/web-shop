using System.Collections.Generic;

namespace API.Entities
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<City> Cities { get; set; }
        //public ICollection<Canton> Cantons { get; set; }
    }
}
