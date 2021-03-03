using Abp.Application.Services;
using ThemePark.Application.AgentTicket.Dto;
using System.Collections.Generic;
using ThemePark.ApplicationDto;

namespace ThemePark.Application.AgentTicket.Interfaces
{
    public interface ITravelOrderConfirmAppService : IApplicationService
    {
        /// <summary>
        /// 增加订单确认记录
        /// </summary>
        /// <param name="dto"></param>
        void AddTravelOrderConfirm(TravelOrderConfirmDto dto);

        /// <summary>
        /// 增加订单确认记录并返回该实体Dto
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        TravelOrderConfirmDto AddAndReturnTravelOrderConfirm(TravelOrderConfirmDto dto);

        /// <summary>
        /// 更新订单确认记录
        /// </summary>
        /// <param name="dto"></param>
        void UpdateTravelOrderConfirm(TravelOrderConfirmDto dto);

        /// <summary>
        /// 删除订单确认记录
        /// </summary>
        /// <param name="deleteInput"></param>
        void DeleteTravelOrderConfirm(DeleteInput deleteInput);

        /// <summary>
        /// 获取订单确认
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        TravelOrderConfirmDto GetTravelOrderConfirm(TravelOrderConfirmDto dto);

        /// <summary>
        /// 根据订单头Id获取订单确认记录
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        IList<TravelOrderConfirmDto> GetByTOHeaderId(TravelOrderConfirmDto dto);

    }
}
