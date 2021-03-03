using System.Linq;
using Abp.Application.Services;
using ThemePark.Application.BasicTicketType.Dto;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using ThemePark.ApplicationDto;
using ThemePark.Infrastructure.Application;
using ThemePark.Core.BasicTicketType;
using System.Threading.Tasks;
using Abp.Domain.Entities.Auditing;

namespace ThemePark.Application.BasicTicketType.Interfaces
{
    /// <summary>
    /// 门市价应用服务层接口
    /// </summary>
    public interface IParkTicketStandardPriceAppService : IApplicationService
    {
        /// <summary>
        /// 增加公园基础票类门市价
        /// </summary>
        /// <param name="dto"></param>
        Task<Result> AddStandardPriceListAsync(List<ParkTicketStandardPriceDto> dto);

        /// <summary>
        /// 删除公园基础票类门市价
        /// </summary>
        /// <param name="id"></param>
        Task<Result> DeleteParkTicketStandardPriceAsync(int id);

        /// <summary>
        /// 更新公园基础票类门市价
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="input"></param>
        Task<Result> UpdateParkTicketStandardPriceAsync(int Id, ParkTicketStandardPriceUpdateinput input);

        /// <summary>
        /// 获取公园基础票类门市价
        /// </summary>
        /// <param name="parameter">页面查询的参数</param>
        /// <returns>页面查询结果</returns>
        Task<PageResult<ParkTicketStandardPriceDto>> GetParkTicketStandardPriceAsync(QueryParameter<ParkTicketStandardPrice> parameter);

        /// <summary>
        /// 根据查询条件获取公园票种门市价信息列表
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="parkTicketStandardPricePageQuery"></param>
        /// <returns></returns>
        Task<PageResult<TDto>> GetAllByPageAsync<TDto>(IPageQuery<ParkTicketStandardPrice> parkTicketStandardPricePageQuery);

        /// <summary>
        /// 根据条件获取公园基础票类门市价列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IList<Dto>> GetParkTicketStandardPriceListAsync<Dto>(IQuery<ParkTicketStandardPrice> query);

        /// <summary>
        /// 根据条件获取公园基础票类门市价
        /// </summary>
        Task<TDto> GetParkTicketStandardPriceAsync<TDto>(IQuery<ParkTicketStandardPrice> query);
    }
}

