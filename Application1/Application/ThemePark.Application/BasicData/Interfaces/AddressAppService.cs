using Abp.Application.Services;
using System.Threading.Tasks;
using ThemePark.ApplicationDto.BasicData;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.BasicData.Interfaces
{
    /// <summary>
    /// Interface IAddressAppService
    /// </summary>
    /// <seealso cref="Abp.Application.Services.IApplicationService"/>
    public interface IAddressAppService : IApplicationService
    {
        #region Methods

        /// <summary>
        /// 新增地址
        /// </summary>
        /// <param name="input"></param>
        Task<Result<AddressDto>> AddAddressAsync(AddAddressInput input);

        /// <summary>
        /// 删除地址信息
        /// </summary>
        /// <param name="addressId">The address identifier.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        Task<Result> DeleteAddressAsync(int addressId);

        /// <summary>
        /// 更新地址信息
        /// </summary>
        /// <param name="input"></param>
        Task<Result<AddressDto>> UpdateAddressIfModifyAsync(UpdateAddressInput input);

        #endregion Methods
    }
}