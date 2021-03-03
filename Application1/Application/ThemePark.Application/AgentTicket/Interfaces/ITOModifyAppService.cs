using Abp.Application.Services;
using ThemePark.Application.AgentTicket.Dto;
using System.Collections.Generic;
using ThemePark.ApplicationDto;

namespace ThemePark.Application.AgentTicket.Interfaces
{
    public interface ITOModifyAppService : IApplicationService
    {
        /// <summary>
        /// 增加订单修改记录
        /// </summary>
        /// <param name="dto"></param>
        void AddTOModify(TOModifyDto dto);

        /// <summary>
        /// 增加订单修改记录并返回该实体Dto
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        TOModifyDto AddAndReturnTOModify(TOModifyDto dto);

        /// <summary>
        /// 更新订单修改记录
        /// </summary>
        /// <param name="dto"></param>
        void UpdateTOModify(TOModifyDto dto);

        /// <summary>
        /// 删除订单修改记录
        /// </summary>
        /// <param name="deleteInput"></param>
        void DeleteTOModify(DeleteInput deleteInput);

        /// <summary>
        /// 获取订单修改记录
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        TOModifyDto GetTOModify(TOModifyDto dto);

        /// <summary>
        /// 根据订单头Id获取订单修改记录
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        IList<TOModifyDto> GetByHeaderId(TOModifyDto dto);
    }
}
