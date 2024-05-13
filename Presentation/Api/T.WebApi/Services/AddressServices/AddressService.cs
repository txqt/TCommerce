using T.Library.Model.Common;
using T.Library.Model.Interface;
using T.WebApi.Services.IRepositoryServices;

namespace T.WebApi.Services.AddressServices
{
    public interface IAddressService : IAddressServiceCommon
    {
        Task<DeliveryAddress?> GetAddressByIdAsync(int id);
    }
    public class AddressService : IAddressService
    {
        private readonly IRepository<DeliveryAddress> _addressRepository;

        public AddressService(IRepository<DeliveryAddress> addressRepository)
        {
            _addressRepository = addressRepository;
        }

        public async Task<DeliveryAddress?> GetAddressByIdAsync(int id)
        {
            return await _addressRepository.GetByIdAsync(id);
        }
    }
}
