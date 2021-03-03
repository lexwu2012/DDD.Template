using Abp.Application.Services;
using System.Collections.Generic;
using ThemePark.Application.AgentTicket.Dto;
using ThemePark.ApplicationDto;

namespace ThemePark.Application.AgentTicket.Interfaces
{
    public interface ITOMssageAppService : IApplicationService
    {
        /// <summary>
        /// 增加门票订单短信
        /// </summary>
        /// <param name="dto"></param>
        void AddTOModify(TOMessageDto dto);

        /// <summary>
        /// 增加门票订单短信并返回该实体Dto
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        TOMessageDto AddAndReturnTOModify(TOMessageDto dto);

        /// <summary>
        /// 更新门票订单短信
        /// </summary>
        /// <param name="dto"></param>
        void UpdateTOModify(TOMessageDto dto);

        /// <summary>
        /// 删除门票订单短信
        /// </summary>
        /// <param name="deleteInput"></param>
        void DeleteTOModify(DeleteInput deleteInput);

        /// <summary>
        /// 获取门票订单短信
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        TOMessageDto GetTOModify(TOMessageDto dto);

        /// <summary>
        /// 根据订单头Id获取门票订单短信
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        IList<TOMessageDto> GetByHeaderId(TOMessageDto dto);

        /// <summary>
        /// 根据顾客Id获取门票订单短信
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        IList<TOMessageDto> GetByCustomerId(TOMessageDto dto);
    }
}
