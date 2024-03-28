using T.Library.Model.Common;
using T.Library.Model.Interface;
using T.WebApi.Extensions;
using T.WebApi.Services.IRepositoryServices;

namespace T.WebApi.Services.StateServices
{
    public interface IStateService : IStateServiceCommon
    {

    }
    public class StateService : IStateService
    {
        private readonly IRepository<State> _stateRepository;

        public StateService(IRepository<State> stateRepository)
        {
            _stateRepository = stateRepository;
        }

        public async Task DeleteStateAsync(int id)
        {
            await _stateRepository.DeleteAsync(id);
        }

        public async Task<State> GetStateByIdAsync(int stateId)
        {
            return await _stateRepository.GetByIdAsync(stateId);
        }

        public Task<State> GetStateByAbbreviationAsync(string abbreviation, int? countryId = null)
        {
            throw new NotImplementedException();
        }

        public async Task<State> GetStateByAddressAsync(Address address)
        {
            return await (await GetStatesAsync()).FirstOrDefaultAsync(x => x.Id == address.StateId);
        }

        public async Task<List<State>> GetStatesByCountryIdAsync(int countryId, bool showHidden = false)
        {
            var states = await GetStatesAsync();
            return states.Where(x=>x.Hidden == showHidden).ToList();
        }

        public async Task<List<State>> GetStatesAsync(bool showHidden = false)
        {
            return (await _stateRepository.GetAllAsync()).Where(c => !c.Hidden).ToList();
        }

        public async Task CreateStateAsync(State state)
        {
            await _stateRepository.CreateAsync(state);
        }

        public async Task UpdateStateAsync(State state)
        {
            await _stateRepository.UpdateAsync(state);
        }

        public async Task BulkCreateStateAsync(List<State> states)
        {
            await _stateRepository.BulkCreateAsync(states);
        }
    }
}
