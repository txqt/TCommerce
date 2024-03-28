using T.Library.Model.Common;
using T.Library.Model.Interface;
using T.WebApi.Extensions;
using T.WebApi.Services.IRepositoryServices;
using static NodaTime.TimeZones.TzdbZone1970Location;

namespace T.WebApi.Services.CityServices
{
    public interface ICityService : ICityServiceCommon
    {

    }
    public class CityService : ICityService
    {
        private readonly IRepository<City> _cityRepository;

        public CityService(IRepository<City> cityRepository)
        {
            _cityRepository = cityRepository;
        }

        public async Task DeleteCityAsync(int id)
        {
            await _cityRepository.DeleteAsync(id);
        }

        public async Task<City> GetCityByIdAsync(int cityId)
        {
            return await _cityRepository.GetByIdAsync(cityId);
        }

        public async Task<City> GetCityByAddressAsync(Address address)
        {
            var countries = await GetCitysAsync(showHidden: true);

            return await countries.FirstOrDefaultAsync(x => x.Id == address.Id);
        }

        public async Task<List<City>> GetCitysByCountryIdAsync(int countryId, bool showHidden = false)
        {
            return (await GetCitysAsync()).Where(x=>x.CountryId == countryId && x.Hidden == showHidden).ToList();
        }

        public async Task<List<City>> GetCitysByStateIdAsync(int stateId, bool showHidden = false)
        {
            return (await GetCitysAsync()).Where(x => x.StateId == stateId && x.Hidden == showHidden).ToList();
        }

        public async Task<List<City>> GetCitysAsync(bool showHidden = false)
        {
            return (await _cityRepository.GetAllAsync()).Where(x=>x.Hidden == showHidden).ToList();
        }

        public async Task CreateCityAsync(City city)
        {
            await _cityRepository.CreateAsync(city);
        }

        public async Task UpdateCityAsync(City city)
        {
            await _cityRepository.UpdateAsync(city);
        }

        public async Task BulkCreateCityAsync(List<City> cities)
        {
            await _cityRepository.BulkCreateAsync(cities);
        }
    }
}
