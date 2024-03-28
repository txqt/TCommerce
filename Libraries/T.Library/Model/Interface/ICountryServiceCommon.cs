using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T.Library.Model.Common;
using T.Library.Model.Response;

namespace T.Library.Model.Interface
{
    public interface ICountryServiceCommon
    {
        Task<List<Country>> GetAllCountriesAsync(bool showHidden = false);
        Task<List<Country>> GetAllCountriesForBillingAsync(bool showHidden = false);
        Task<List<Country>> GetAllCountriesForShippingAsync(bool showHidden = false);
        Task<Country> GetCountryByAddressAsync(Address address);
        Task<Country> GetCountryByIdAsync(int countryId);
        Task<List<Country>> GetCountriesByIdsAsync(int[] countryIds);
        Task<Country> GetCountryByTwoLetterIsoCodeAsync(string twoLetterIsoCode);
        Task<Country> GetCountryByThreeLetterIsoCodeAsync(string threeLetterIsoCode);
        Task<ServiceResponse<bool>> CreateCountryAsync(Country country);
        Task<ServiceResponse<bool>> BulkCreateCountryAsync(List<Country> countries);
        Task<ServiceResponse<bool>> UpdateCountryAsync(Country country);
    }
}
