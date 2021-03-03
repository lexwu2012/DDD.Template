using Abp.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using ThemePark.Core.BasicData;
using ThemePark.Application.AgentTicket.Dto;
using Abp.AutoMapper;
using ThemePark.Application.AgentTicket.Interfaces;
using ThemePark.ApplicationDto;

namespace ThemePark.Application.AgentTicket
{
    public class GroupMembersAppService : ThemeParkAppServiceBase, IGroupMembersAppService
    {
        #region Fields

        private readonly IRepository<GroupMembers, long> _groupMembersRepository;

        #endregion

        #region Ctor

        public GroupMembersAppService(IRepository<GroupMembers, long> groupMembersRepository)
        {
            _groupMembersRepository = groupMembersRepository;
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// 增加旅游团成员
        /// </summary>
        /// <param name="dto"></param>
        public void AddGroupMember(GroupMembersDto dto)
        {
            var groupMember = dto.MapTo<GroupMembers>();

            _groupMembersRepository.Insert(groupMember);
        }

        /// <summary>
        /// 增加旅游团成员并返回该实体Dto
        /// </summary>
        /// <param name="dto"></param>
        public GroupMembersDto AddAndReturnGroupMember(GroupMembersDto dto)
        {
            var groupMember = dto.MapTo<GroupMembers>();

            _groupMembersRepository.Insert(groupMember);

            UnitOfWorkManager.Current.SaveChanges();

            return groupMember.MapTo<GroupMembersDto>();
        }

        /// <summary>
        /// 删除旅游团成员
        /// </summary>
        /// <param name="dto"></param>
        public void DeleteGroupMember(DeleteInput input)
        {
            _groupMembersRepository.Delete(g => g.Id == input.Id);
        }

        /// <summary>
        /// 更新旅游团成员
        /// </summary>
        /// <param name="dto"></param>
        public void UpdateGroupMember(GroupMembersDto dto)
        {
            _groupMembersRepository.Update(dto.MapTo<GroupMembers>());
        }

        /// <summary>
        /// 根据顾客Id获取旅游团成员
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public IList<GroupMembersDto> GetByCustomerId( GroupMembersDto dto)
        {
            var GroupMembers = _groupMembersRepository.GetAll().Where( g => g.CustomerId == dto.CustomId );

            return GroupMembers.MapTo<IList<GroupMembersDto>>();
        }

        /// <summary>
        /// 根据团体信息Id获取旅游团成员
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public IList<GroupMembersDto> GetByGroupInfoId( GroupMembersDto dto)
        {
            var GroupMembers = _groupMembersRepository.GetAll().Where( g => g.GroupInfoId == dto.GroupInfoId );

            return GroupMembers.MapTo<IList<GroupMembersDto>>();
        }

        /// <summary>
        /// 获取旅游团成员
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public GroupMembersDto GetGroupMember( GroupMembersDto dto)
        {
            var GroupMember = _groupMembersRepository.Get( dto.Id );

            return GroupMember.MapTo<GroupMembersDto>();
        }

        #endregion
    }
}
