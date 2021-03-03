using Abp.Application.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThemePark.Application.Agencies.Dto;
using ThemePark.ApplicationDto.Agencies;
using ThemePark.Core.Agencies;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.Agencies.Interfaces
{ 
    /// <summary>
    /// 
    /// </summary>
    public interface IAgencyTypeAppService : IApplicationService
    {

        /// <summary>
        /// 增加新的代理商类型，异步方法
        /// </summary>
        Task<Result> AddAgencyTypeAsync(AddAgencyTypeInput input);

        /// <summary>
        /// 更新代理商类型信息，异步方法
        /// </summary>
        Task<Result> UpdateAgencyTypeAsync(int id,UpdateAgencyTypeInput input);


        /// <summary>
        /// 更新出票类型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result> UpdateAgencyTypeAsync(AgencyTypeOutTicketTypeInput input);


        /// <summary>
        /// 根据Id删除代理商类型，异步方法
        /// </summary>
        Task<Result> DeleteAgencyTypeAsync(int id);

        /// <summary>
        /// 根据ID获取代理商类型
        /// </summary>
        /// <returns></returns>
        Task<AgencyTypeDto> GetAgencyTypeByIdAsync(int id);

        /// <summary>
        /// 获取代理商列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IList<TDto>> GetAgencyTypeListAsync<TDto>(IQuery<AgencyType> query);

        /// <summary>
        /// 获取代理商类型
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<TDto> GetAgencyTypeAsync<TDto>(IQuery<AgencyType> query);

        /// <summary>
        /// 获取出票类型
        /// </summary>
        /// <returns></returns>
        Task<AgencyTypeOutTicketTypeDto> GetAgencyOutTicketTypeAsync(int agencyId);

        /// <summary>
        /// DataTable查询获取代理商类型
        /// </summary>
        /// <typeparam name="TDto">The type of the t dto.</typeparam>
        /// <param name="agencyTypePageQuery">The agency type page query.</param>
        Task<PageResult<TDto>> GetAllByPageAsync<TDto>(IPageQuery<AgencyType> agencyTypePageQuery);

        /// <summary>
        /// 获取所有的代理商类型
        /// </summary>
        /// <returns></returns>
        IQueryable<AgencyTypeWithDefaultType> GetAgencyTypesAsync();

        /// <summary>
        /// 获取所有代理商下拉列表
        /// </summary>
        /// <returns></returns>
        Task<DropdownDto> GetAgencyTypesDropdownAsync();

        /// <summary>
        /// 获取(ota、自有渠道）代理商类型下拉列表
        /// </summary>
        /// <returns></returns>
        Task<DropdownDto> GetAgencyTypesDropdownByCache2Async();

        /// <summary>
        /// 获取所有代理商类型下拉列表
        /// </summary>
        /// <returns></returns>
        Task<DropdownDto> GetAgencyTypesDropdown4WindowAsync();
    }
}