using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThemePark.Application.CardManagement.Dto;
using ThemePark.Core.CardManage;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.CardManagement.Interfaces
{
    /// <summary>
    /// IC卡初始化应用服务接口
    /// </summary>
    public interface ICardActivationAppService : IApplicationService
    {
        /// <summary>
        /// 添加IC卡基础信息记录
        /// </summary>
        /// <returns></returns>
        Task<Result> AddIcBasicInfoAsync(int parkId, IList<IcBasicInfoInput> inputs);

        /// <summary>
        /// 根据条件获取IC卡基础信息记录
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<TDto> GetIcBasicInfoAsync<TDto>(IQuery<IcBasicInfo> query);

        /// <summary>
        /// 根据条件获取IC卡基础信息记录列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IList<TDto>> GetIcBasicInfoListAsync<TDto>(IQuery<IcBasicInfo> query);

        /// <summary>
        /// 添加管理卡领用记录
        /// </summary>
        /// <returns></returns>
        Task<Result> AddManageCardRequisitionAsync(CardRequisitionInput input);

        /// <summary>
        /// 根据条件查询管理卡领用信息
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<TDto> GetManageCardRequisitionAsync<TDto>(IQuery<ManageCardInfo> query);

        /// <summary>
        /// 获取管理卡和年卡分页数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<PageResult<IcBasicInfoDetailDto>> GetAllByPageAsync(AccurateSearchModel query = null);
    }
}
