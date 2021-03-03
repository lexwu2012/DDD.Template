using Abp.Application.Services;
using ThemePark.Application.AgentTicket.Dto;
using System.Collections.Generic;
using ThemePark.ApplicationDto;

namespace ThemePark.Application.AgentTicket.Interfaces
{
    public interface ITORefundAppService : IApplicationService
    {
        /// <summary>
        /// 增加订单退款
        /// </summary>
        /// <param name="dto"></param>
        void AddTORefund(TORefundDto dto);

        /// <summary>
        /// 增加订单退款记录并返回该实体Dto
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        TORefundDto AddAndReturnTORefund(TORefundDto dto);

        /// <summary>
        /// 更新订单退款记录
        /// </summary>
        /// <param name="dto"></param>
        void UpdateTORefund(TORefundDto dto);

        /// <summary>
        /// 删除订单退款
        /// </summary>
        /// <param name="deleteInput"></param>
        void DeleteTORefund(DeleteInput deleteInput);

        /// <summary>
        /// 获取订单退款
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        TORefundDto GetTORefund(TORefundDto dto);

        /// <summary>
        /// 根据修改记录Id获取订单退款
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        IList<TORefundDto> GetByModifyId(TORefundDto dto);

        /// <summary>
        /// 根据订单头Id获取订单退款
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        IList<TORefundDto> GetByHeaderId(TORefundDto dto);
    }
}
