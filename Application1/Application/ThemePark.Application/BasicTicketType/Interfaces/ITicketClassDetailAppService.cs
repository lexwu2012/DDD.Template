using System.Linq;
using Abp.Application.Services;
using ThemePark.Application.BasicTicketType.Dto;
using ThemePark.ApplicationDto;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThemePark.ApplicationDto.BasicTicketType;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.BasicTicketType.Interfaces
{
    public interface ITicketClassDetailAppService : IApplicationService
    {
      
        /// <summary>
        /// 增加票类详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result> AddTicketClassDetailAsync(GetTicketClassDetailPageInput input);

     

        /// <summary>
        /// 根据TicketClassId删除票类详情
        /// </summary>
        /// <param name="ticketClassID"></param>
        Task<Result> DeleteByTicketClassIdAsync(int ticketClassID);


        /// <summary>
        /// 根据TicketClassId获取基础票类详情
        /// </summary>
        /// <param name="ticketClassID"></param>
        Task<IList<TicketClassDetailDto>> GetTicketClassDetailsByTicketClassIdAsync(int ticketClassID);


        /// <summary>
        ///  根据票类Id获取不重复的基础票类
        /// </summary>
        /// <param name="ticketClassID"></param>
        /// <returns></returns>
        Task<IList<TicketTypeDto>> GetTicketTypesByTicketClassIdAsync(int ticketClassID);
    }
}

