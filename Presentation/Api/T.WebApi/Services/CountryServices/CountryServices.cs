using Newtonsoft.Json;
using System.Linq;
using T.Library.Model.Common;
using T.Library.Model.Interface;
using T.Library.Model.Response;
using T.WebApi.Extensions;
using T.WebApi.Services.IRepositoryServices;

namespace T.WebApi.Services.CountryServices
{
    public interface ICountryService : ICountryServiceCommon
    {

    }
    public class CountryService : ICountryService
    {
        private readonly IRepository<Country> _countryRepository;

        public CountryService(IRepository<Country> countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public async Task<List<Country>> GetAllCountriesAsync(bool showHidden = false)
        {
            var countries = (await _countryRepository.GetAllAsync()).ToList();

            if (!showHidden)
            {
                countries = countries.Where(c => !c.Hidden).ToList();
            }

            return countries;
        }

        public async Task<List<Country>> GetAllCountriesForBillingAsync(bool showHidden = false)
        {
            var countries = await GetAllCountriesAsync(showHidden);
            return countries.Where(c => c.AllowBilling).ToList();
        }

        public async Task<List<Country>> GetAllCountriesForShippingAsync(bool showHidden = false)
        {
            var countries = await GetAllCountriesAsync(showHidden);
            return countries.Where(c => c.AllowShipping).ToList();
        }

        public async Task<Country> GetCountryByAddressAsync(Address address)
        {
            var countries = await GetAllCountriesAsync(showHidden: true);

            return await countries.FirstOrDefaultAsync(x => x.Id == address.Id);
        }

        public async Task<Country> GetCountryByIdAsync(int countryId)
        {
            var countries = await GetAllCountriesAsync(showHidden: true);
            return countries.FirstOrDefault(c => c.Id == countryId);
        }

        public async Task<List<Country>> GetCountriesByIdsAsync(int[] countryIds)
        {
            var countries = await GetAllCountriesAsync(showHidden: true);
            return countries.Where(c => countryIds.Any(id => id == c.Id)).ToList();
        }

        public async Task<Country> GetCountryByTwoLetterIsoCodeAsync(string twoLetterIsoCode)
        {
            var countries = await GetAllCountriesAsync(showHidden: true);
            return countries.FirstOrDefault(c => c.Iso2 == twoLetterIsoCode);
        }

        public async Task<Country> GetCountryByThreeLetterIsoCodeAsync(string threeLetterIsoCode)
        {
            var countries = await GetAllCountriesAsync(showHidden: true);
            return countries.FirstOrDefault(c => c.Iso3 == threeLetterIsoCode);
        }

        public async Task<ServiceResponse<bool>> CreateCountryAsync(Country country)
        {
            await _countryRepository.CreateAsync(country);
            return new ServiceSuccessResponse<bool>();
        }
        public async Task<ServiceResponse<bool>> UpdateCountryAsync(Country country)
        {
            await _countryRepository.UpdateAsync(country);
            return new ServiceSuccessResponse<bool>();
        }

        public async Task<ServiceResponse<bool>> BulkCreateCountryAsync(List<Country> countries)
        {
            await _countryRepository.BulkCreateAsync(countries);
            return new ServiceSuccessResponse<bool>();
        }
    }
}
