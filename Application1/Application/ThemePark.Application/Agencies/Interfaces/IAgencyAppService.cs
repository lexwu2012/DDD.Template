using System;
using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThemePark.Application.Agencies.Dto;
using ThemePark.Core.Agencies;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.Agencies.Interfaces
{
    public interface IAgencyAppService: IApplicationService
    {
        /// <summary>
        /// 增加新的代理商
        /// </summary>
        /// <param name="input"></param>
        Task<Result> AddAgencyAsync(AddAgencyInput input);
        
        /// <summary>
        /// 获取所有代理商
        /// </summary>
        /// <returns></returns>
        Task<DropdownDto> GetAllAgenciesDropdownAsync(IQuery<Agency> query = null);

        /// <summary>
        /// 更新代理商信息
        /// </summary>
        Task<Result> UpdateAgencyAsync(int id,UpdateAgencyInput input);

        /// <summary>
        /// 代理商信息查询
        /// </summary>
        /// <param name="agencyPageQuery">查询条件</param>
        /// <returns>查询结果</returns>
        Task<PageResult<TDto>> GetAllByPageAsync<TDto>(IPageQuery<Agency> agencyPageQuery);

        /// <summary>
        /// 获取代理商信息
        /// </summary>
        Task<TDto> GetAgencyAsync<TDto>(IQuery<Agency> query);

        /// <summary>
        /// 获取代理商信息
        /// </summary>
        Task<List<TDto>> GetAgencyListAsync<TDto>(IQuery<Agency> query);

        /// <summary>
        /// 获取代理商下拉列表
        /// </summary>
        /// <returns></returns>
        Task<DropdownDto> GetPreDeposiAgencyListAsync();

        /// <summary>
        /// 订单确认更改电话
        /// </summary>
        /// <param name="id"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        Task UpdateAgencyPhoneAsync(int id, string phone);

        /// <summary>
        /// 批量更新账户有效期
        /// </summary>
        /// <param name="parkAgencyIds"></param>
        /// <param name="startDateTime"></param>
        /// <param name="expirationDateTime"></param>
        /// <returns></returns>
        Result BulkUpdateParkAgencyExpiredDate(List<int> parkAgencyIds, DateTime startDateTime,
            DateTime expirationDateTime);

        /// <summary>
        /// 删除代理商
        /// </summary>
        /// <returns></returns>
        Task<Result> DeleteAgencyAsync(int id);
    }
}
