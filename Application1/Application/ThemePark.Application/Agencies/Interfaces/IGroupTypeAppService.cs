using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThemePark.Application.Agencies.Dto;
using ThemePark.ApplicationDto.Agencies;
using ThemePark.Core.Agencies;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.Agencies.Interfaces
{
    /// <summary>
    /// 可带队类型IService
    /// </summary>
    public interface IGroupTypeAppService : IApplicationService
    {
        /// <summary>
        /// 增加新的带队类型,并返回该新增实体
        /// </summary>
        Task<Result<GroupTypeDto>> AddGroupTypeAsync(GroupTypeInput input);

        /// <summary>
        /// 更新带队类型信息
        /// </summary>
        Task<Result> UpdateGroupTypeAsync(int id, GroupTypeInput input);

        /// <summary>
        /// 根据Id删除带队类型
        /// </summary>
        Task<Result> DeleteGroupTypeByIdAsync(int id);

        /// <summary>
        /// 获取带队类型下拉列表
        /// </summary>
        /// <returns>Task&lt;DropdownDto&gt;.</returns>
        Task<DropdownDto> GetGroupTypesDropdownAsync();

        /// <summary>
        /// 获取所有的带队类型
        /// </summary>
        /// <returns></returns>
        Task<DropdownDto> GetAllGroupTypesAsync();

        /// <summary>
        /// 获取当前登录代理商的带队类型
        /// </summary>
        /// <returns>Task&lt;DropdownDto&gt;.</returns>
        Task<DropdownDto> GetAgencyGroupTypesDropdownAsync(int? agencyId);

        /// <summary>
        /// 获取列表展示内容
        /// </summary>
        /// <param name="groupTypePageQuery"></param>
        /// <returns></returns>
        Task<PageResult<TDto>> GetAllByPageAsync<TDto>(IPageQuery<GroupType> groupTypePageQuery);

        /// <summary>
        /// 查询带队类型列表
        /// </summary>
        Task<IList<TDto>> GetGroupTypeListAsync<TDto>(IQuery<GroupType> query=null);


        /// <summary>
        /// 查询带队类型
        /// </summary>
        Task<TDto> GetGroupTypeAsync<TDto>(IQuery<GroupType> query);
    }
}
