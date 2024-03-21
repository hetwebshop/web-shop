using API.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Data
{
    public interface ILocationRepository
    {
        Task<List<City>> GetAllCitiesAsync();
        Task<List<Country>> GetAllCountriesAsync();
        //Task<List<Municipality>> GetAllMunicipalitiesAsync();
        //Task<List<Canton>> GetAllCantonsAsync();
    }
}
