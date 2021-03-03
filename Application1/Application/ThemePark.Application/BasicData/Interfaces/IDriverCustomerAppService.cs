using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThemePark.Application.BasicData.Dto;
using ThemePark.Core.BasicData;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.BasicData.Interfaces
{
    /// <summary>
    /// 司机应用服务接口
    /// </summary>
    public interface IDriverCustomerAppService : IApplicationService
    {
        /// <summary>
        /// 添加司陪信息
        /// </summary>
        Task<Result<int>> AddDriverCustomerAsync(DriverCustomerInput input);
        /// <summary>
        /// 修改司陪信息
        /// </summary>
        Task<Result> UpdateDriverCustomerAsync(DriverCustomerInput input, int agencyId);
        /// <summary>
        /// 删除司陪信息
        /// </summary>
        Task<Result> DelDriverCustomerAsync(int id, int agencyId);

        /// <summary>
        /// 查询司陪信息
        /// </summary>
        Task<IList<DriverCustomerDto>> GetDriverCustomerAsync(int agencyId, IEnumerable<int> driverIds = null);
        /// <summary>
        /// 查询司陪信息
        /// </summary>
        Task<PageResult<TDto>> GetDriverCustomerAsync<TDto>(IPageQuery<DriverCustomer> query);
        /// <summary>
        /// 查询司陪信息
        /// </summary>
        Task<IList<TDto>> GetDriverCustomerListAsync<TDto>(IQuery<DriverCustomer> query);
    }
}
