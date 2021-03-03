using Abp.AutoMapper;
using Abp.Domain.Repositories;
using System.Threading.Tasks;
using ThemePark.Application.AgentTicket.Dto;
using ThemePark.Application.AgentTicket.Interfaces;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.AgentTicket
{
    /// <summary>
    /// 团体信息应用服务（旅行社添加订单时这个团体信息是作为中间表关联代理商和司机、导游，OTA订单不会生成这个信息）
    /// </summary>
    public class GroupInfoAppService : ThemeParkAppServiceBase, IGroupInfoAppService
    {
        #region Fields
        private readonly IRepository<GroupInfo, long> _groupInfoRepository;
        #endregion

        #region Ctor
        public GroupInfoAppService(IRepository<GroupInfo, long> groupInfoRepository)
        {
            _groupInfoRepository = groupInfoRepository;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// 获取团体信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public GroupInfoDto GetGroupInfo(GroupInfoDto dto)
        {
            var gf = _groupInfoRepository.Get(dto.Id);

            if(gf != null)
            {
                return gf.MapTo<GroupInfoDto>();
            }

            return null;
        }

        /// <summary>
        /// 增加团体信息
        /// </summary>
        /// <param name="dto"></param>
        public void AddGroupInfo(GroupInfoDto dto)
        {
            var groupInfo = dto.MapTo<GroupInfo>();

            _groupInfoRepository.Insert(groupInfo);
        }

        /// <summary>
        /// 增加团体信息并返回该实体Dto
        /// </summary>
        /// <param name="dto"></param>
        public GroupInfoDto AddAndReturnGroupInfo(GroupInfoDto dto)
        {
            var groupInfo = dto.MapTo<GroupInfo>();

            _groupInfoRepository.Insert(groupInfo);

            UnitOfWorkManager.Current.SaveChanges();

            return groupInfo.MapTo<GroupInfoDto>();
        }

        /// <summary>
        /// 更新团体信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        public async Task UpdateGroupInfo(long id,GroupInfoDto dto)
        {
            await _groupInfoRepository.UpdateAsync(id,m => Task.FromResult(dto.MapTo(m)));
        }

        #endregion
    }
}
