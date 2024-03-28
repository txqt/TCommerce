using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T.Library.Model.Common;

namespace T.Library.Model.Interface
{
    public interface ICityServiceCommon
    {
        Task DeleteCityAsync(int id);
        Task<City> GetCityByIdAsync(int cityId);
        Task<City> GetCityByAddressAsync(Address address);
        Task<List<City>> GetCitysByCountryIdAsync(int countryId, bool showHidden = false);
        Task<List<City>> GetCitysByStateIdAsync(int stateId, bool showHidden = false);
        Task<List<City>> GetCitysAsync(bool showHidden = false);
        Task CreateCityAsync(City city);
        Task BulkCreateCityAsync(List<City> cities);
        Task UpdateCityAsync(City city);
    }
}
