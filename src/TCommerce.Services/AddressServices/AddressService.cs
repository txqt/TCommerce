﻿using TCommerce.Core.Helpers;
using TCommerce.Core.Interface;
using TCommerce.Core.Models.Common;
using TCommerce.Services.IRepositoryServices;

namespace TCommerce.Services.AddressServices
{
    public class AddressService : IAddressService
    {
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<VietNamProvince> _vietNamProvinceRepository;
        private readonly IRepository<VietNamDistrict> _vietNamDistrictRepository;
        private readonly IRepository<VietNamCommune> _vietNamCommuneRepository;

        public AddressService(IRepository<Address> addressRepository, IRepository<VietNamProvince> vietNamProvinceRepository, IRepository<VietNamDistrict> vietNamDistrictRepository, IRepository<VietNamCommune> vietNamCommuneRepository)
        {
            _addressRepository = addressRepository;
            _vietNamProvinceRepository = vietNamProvinceRepository;
            _vietNamDistrictRepository = vietNamDistrictRepository;
            _vietNamCommuneRepository = vietNamCommuneRepository;
        }

        public async Task<Address?> GetAddressByIdAsync(int id)
        {
            return await _addressRepository.GetByIdAsync(id);
        }

        public async Task BulkCreateProvince(IEnumerable<VietNamProvince> provinceList)
        {
            await _vietNamProvinceRepository.BulkCreateAsync(provinceList);
        }

        public async Task BulkCreateDistrict(IEnumerable<VietNamDistrict> districtList)
        {
            await _vietNamDistrictRepository.BulkCreateAsync(districtList);
        }

        public async Task BulkCreateCommune(IEnumerable<VietNamCommune> communeList)
        {
            await _vietNamCommuneRepository.BulkCreateAsync(communeList);
        }

        public async Task<List<VietNamProvince>> GetAllProvinces()
        {
            return (await _vietNamProvinceRepository.GetAllAsync()).OrderBy(x => x.Name).ToList();
        }

        public async Task<List<VietNamDistrict>> GetDistrictsByProvinceId(int provinceId)
        {
            var result = await _vietNamDistrictRepository.GetAllAsync(func: query =>
            {
                query = from d in _vietNamDistrictRepository.Table
                        where d.IdProvince == provinceId
                        select d;
                return query;
            }, cacheKey: CacheKeysDefault<VietNamDistrict>.AllPrefix + $"_id:{provinceId}");

            return result.OrderBy(x => x.Name).ToList();
        }

        public async Task<List<VietNamCommune>> GetCommunesByDistrictId(int districtId)
        {
            var result = await _vietNamCommuneRepository.GetAllAsync(func: query =>
            {
                query = from c in _vietNamCommuneRepository.Table
                        where c.IdDistrict == districtId
                        select c;
                return query;
            }, cacheKey: CacheKeysDefault<VietNamCommune>.AllPrefix + $"_id:{districtId}");

            return result.OrderBy(x => x.Name).ToList();
        }

        public async Task CreateAddressAsync(Address deliveryAddress)
        {
            await _addressRepository.CreateAsync(deliveryAddress);
        }

        public async Task<VietNamProvince?> GetProvinceByIdAsync(int provinceId)
        {
            return await _vietNamProvinceRepository.GetByIdAsync(provinceId);
        }

        public async Task<VietNamDistrict?> GetDistricteByIdAsync(int districtId)
        {
            return await _vietNamDistrictRepository.GetByIdAsync(districtId);
        }

        public async Task<VietNamCommune?> GetCommuneByIdAsync(int communeId)
        {
            return await _vietNamCommuneRepository.GetByIdAsync(communeId);
        }

        public async Task UpdateAddressAsync(Address deliveryAddress)
        {
            await _addressRepository.UpdateAsync(deliveryAddress);
        }

        public async Task DeleteAddressAsync(int id)
        {
            await _addressRepository.DeleteAsync(id);
        }
    }
}
