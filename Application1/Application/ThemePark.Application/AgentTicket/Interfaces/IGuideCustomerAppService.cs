using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThemePark.Application.AgentTicket.Dto;
using ThemePark.Core.BasicData;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.AgentTicket.Interfaces
{
    /// <summary>
    /// 导游应用服务接口
    /// </summary>
    public interface IGuideCustomerAppService : IApplicationService
    {
        /// <summary>
        /// 添加导游信息
        /// </summary>
        Task<Result<int>> AddGuideCustAsync(GuideCustInput input);
        /// <summary>
        /// 修改导游信息
        /// </summary>
        Task<Result> UpdateGuideCustAsync(GuideCustInput input, int agencyId);
        /// <summary>
        /// 删除导游信息
        /// </summary>
        Task<Result> DelGuideCustAsync(int id, int agencyId);
        /// <summary>
        /// 查询导游信息
        /// </summary>
        Task<IList<GuideCustomerDto>> GetAgencyGuidesAsync(int agencyId, IList<int> guideIds = null);

        /// <summary>
        /// 查询导游信息
        /// </summary>
        Task<PageResult<TDto>> GetAgencyGuidesAsync<TDto>(IPageQuery<GuideCustomer> query);

        /// <summary>
        /// 查询导游信息
        /// </summary>
        Task<IList<TDto>> GetGuideCustomerListAsync<TDto>(IQuery<GuideCustomer> query);
    }
}
