using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T.Library.Model.Common;

namespace T.Library.Model.Interface
{
    public interface IStateServiceCommon
    {
        Task DeleteStateAsync(int id);
        Task<State> GetStateByIdAsync(int stateId);
        Task<State> GetStateByAbbreviationAsync(string abbreviation, int? countryId = null);
        Task<State> GetStateByAddressAsync(Address address);
        Task<List<State>> GetStatesByCountryIdAsync(int countryId, bool showHidden = false);
        Task<List<State>> GetStatesAsync(bool showHidden = false);
        Task CreateStateAsync(State state);
        Task BulkCreateStateAsync(List<State> states);
        Task UpdateStateAsync(State state);
    }
}
