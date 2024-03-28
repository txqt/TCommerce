using T.Library.Model.Common;
using T.Library.Model.Interface;
using T.WebApi.Services.IRepositoryServices;

namespace T.WebApi.Services.AddressServices
{
    public interface IAddressService : IAddressServiceCommon
    {
        Task<Address> GetAddressByIdAsync(int id);
    }
    public class AddressService : IAddressService
    {
        private readonly IRepository<Address> _addressRepository;

        public AddressService(IRepository<Address> addressRepository)
        {
            _addressRepository = addressRepository;
        }

        public async Task<Address> GetAddressByIdAsync(int id)
        {
            return await _addressRepository.GetByIdAsync(id);
        }
    }
}
