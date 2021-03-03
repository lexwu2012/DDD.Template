using Abp.Application.Services;
using ThemePark.Application.AgentTicket.Dto;
using System.Collections.Generic;
using ThemePark.ApplicationDto;

namespace ThemePark.Application.AgentTicket.Interfaces
{
    public interface IGroupMembersAppService : IApplicationService
    {
        /// <summary>
        /// 增加旅游团成员
        /// </summary>
        /// <param name="dto"></param>
        void AddGroupMember(GroupMembersDto dto);

        /// <summary>
        /// 增加旅游团成员并返回该实体Dto
        /// </summary>
        /// <param name="dto"></param>
        GroupMembersDto AddAndReturnGroupMember(GroupMembersDto dto);

        /// <summary>
        /// 删除旅游团成员
        /// </summary>
        /// <param name="dto"></param>
        void DeleteGroupMember(DeleteInput input);

        /// <summary>
        /// 更新旅游团成员
        /// </summary>
        /// <param name="dto"></param>
        void UpdateGroupMember(GroupMembersDto dto);

        /// <summary>
        /// 根据顾客Id获取旅游团成员
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        IList<GroupMembersDto> GetByCustomerId(GroupMembersDto dto);

        /// <summary>
        /// 根据团体信息Id获取旅游团成员
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        IList<GroupMembersDto> GetByGroupInfoId(GroupMembersDto dto);

        /// <summary>
        /// 获取旅游团成员
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        GroupMembersDto GetGroupMember(GroupMembersDto dto);
    }
}
