using API.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    public interface ILocationService
    {
        Task<List<City>> GetAllCitiesAsync();
        Task<List<Country>> GetAllCountriesAsync();
        //Task<List<Municipality>> GetAllMunicipalitiesAsync();
        //Task<List<Canton>> GetAllCantonsAsync();
    }
}
