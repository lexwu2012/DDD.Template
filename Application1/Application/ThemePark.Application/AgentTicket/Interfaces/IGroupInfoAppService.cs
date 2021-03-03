using System.Threading.Tasks;
using Abp.Application.Services;
using ThemePark.Application.AgentTicket.Dto;

namespace ThemePark.Application.AgentTicket.Interfaces
{
    /// <summary>
    /// 团体信息应用服务接口
    /// </summary>
    public interface IGroupInfoAppService : IApplicationService
    {
        /// <summary>
        /// 获取团体信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        GroupInfoDto GetGroupInfo(GroupInfoDto dto);

        /// <summary>
        /// 增加团体信息
        /// </summary>
        /// <param name="dto"></param>
        void AddGroupInfo(GroupInfoDto dto);

        /// <summary>
        /// 增加团体信息并返回该实体Dto
        /// </summary>
        /// <param name="dto"></param>
        GroupInfoDto AddAndReturnGroupInfo(GroupInfoDto dto);

        /// <summary>
        /// 更新团体信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        Task UpdateGroupInfo(long id, GroupInfoDto dto);
    }
}
