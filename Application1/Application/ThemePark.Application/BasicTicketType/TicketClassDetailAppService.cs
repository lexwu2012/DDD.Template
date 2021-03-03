using System.Linq;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using ThemePark.Application.BasicTicketType.Dto;
using ThemePark.Application.BasicTicketType.Interfaces;
using ThemePark.Core.BasicTicketType;
using ThemePark.ApplicationDto;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThemePark.Infrastructure.Application;
using ThemePark.Infrastructure.EntityFramework;
using System.Data.Entity;
using ThemePark.ApplicationDto.BasicTicketType;

namespace ThemePark.Application.BasicTicketType
{
    public class TicketClassDetailAppService : ThemeParkAppServiceBase, ITicketClassDetailAppService
    {
        #region Fields

        private readonly IRepository<TicketClassDetail> _ticketClassDetail;

        #endregion

        #region Ctor

        public TicketClassDetailAppService(IRepository<TicketClassDetail> ticketClassDetail)
        {
            _ticketClassDetail = ticketClassDetail;
        }

        #endregion

        #region Public Methods


        /// <summary>
        /// 增加票类详情
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> AddTicketClassDetailAsync(GetTicketClassDetailPageInput input)
        {
            var ticketClassDetail = input.MapTo<TicketClassDetail>();
            await _ticketClassDetail.InsertAsync(ticketClassDetail);
            return Result.Ok();
        }

        /// <summary>
        /// 根据TicketClassId删除票类详情
        /// </summary>
        /// <param name="ticketClassId">TicketClassId</param>
        public async Task<Result> DeleteByTicketClassIdAsync(int ticketClassId)
        {
            await _ticketClassDetail.DeleteAsync(t => t.TicketClassId == ticketClassId);
            return Result.Ok();
        }

        /// <summary>
        /// 根据TicketClassId获取基础票类
        /// </summary>
        /// <param name="ticketClassID"></param>
        public async Task<IList<TicketClassDetailDto>> GetTicketClassDetailsByTicketClassIdAsync(int ticketClassID)
        {
            var entity = await _ticketClassDetail.GetAllListAsync(m => m.TicketClassId == ticketClassID);

            return entity.MapTo<IList<TicketClassDetailDto>>();
        }

        /// <summary>
        /// 根据票类Id获取不重复的基础票类
        /// </summary>
        /// <param name="ticketClassID"></param>
        /// <returns></returns>
        public async Task<IList<TicketTypeDto>> GetTicketTypesByTicketClassIdAsync(int ticketClassID)
        {
            var entity = await _ticketClassDetail.AsNoTrackingAndInclude(m => m.TicketType)
                .Where(m => m.TicketClassId == ticketClassID).ToListAsync();

            IList<TicketType> list = new List<TicketType>();

            foreach (var item in entity)
            {
                if (item.TicketType == null || list.Contains(item.TicketType))
                    continue;
                list.Add(item.TicketType);
            }

            return list.MapTo<IList<TicketTypeDto>>();
        }

        #endregion
    }
}
