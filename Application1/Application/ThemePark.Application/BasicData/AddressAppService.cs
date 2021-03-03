using Abp.AutoMapper;
using System.Threading.Tasks;
using ThemePark.Application.BasicData.Interfaces;
using ThemePark.ApplicationDto.BasicData;
using ThemePark.Core.BasicData;
using ThemePark.Core.BasicData.Repositories;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.BasicData
{
    /// <summary>
    /// 地址服务
    /// </summary>
    public class AddressAppService : IAddressAppService
    {
        #region Fields

        /// <summary>
        /// The _address domain service
        /// </summary>
        private readonly IAddressDomainService _addressDomainService;

        /// <summary>
        /// The _address repository
        /// </summary>
        private readonly IAddressRepository _addressRepository;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// 初始化 <see cref="AddressAppService"/> 类的新实例。
        /// </summary>
        /// <param name="addressRepository">The address repository.</param>
        /// <param name="addressDomainService">The address DomainService.</param>
        public AddressAppService(IAddressRepository addressRepository, IAddressDomainService addressDomainService)
        {
            _addressRepository = addressRepository;
            _addressDomainService = addressDomainService;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// 新增地址
        /// </summary>
        /// <param name="input"></param>
        public async Task<Result<AddressDto>> AddAddressAsync(AddAddressInput input)
        {
            var address = await _addressDomainService.AddAddressAsync(input.ProvinceId, input.CityId,
                input.CountyId, input.StreetId, input.Detail);

            return Result.Ok(address.MapTo<AddressDto>());
        }

        /// <summary>
        /// 删除地址信息
        /// </summary>
        /// <param name="addressId">The address identifier.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        public async Task<Result> DeleteAddressAsync(int addressId)
        {
            var address = await _addressRepository.GetAsync(addressId);
            await _addressDomainService.DeleteAddressAsync(address);

            return Result.Ok();
        }

        /// <summary>
        /// 更新地址信息
        /// </summary>
        /// <param name="input"></param>
        public async Task<Result<AddressDto>> UpdateAddressIfModifyAsync(UpdateAddressInput input)
        {
            var address = await _addressRepository.GetAsync(input.Id);
            await _addressDomainService.UpdateAddressAsync(address, input.ProvinceId, input.CityId, input.CountyId,
                input.StreetId, input.Detail);

            return Result.Ok(address.MapTo<AddressDto>());
        }

        #endregion Methods
    }
}