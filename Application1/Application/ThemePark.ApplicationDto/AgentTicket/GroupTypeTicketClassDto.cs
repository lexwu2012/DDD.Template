using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ThemePark.ApplicationDto.Agencies;
using ThemePark.ApplicationDto.BasicTicketType;
using ThemePark.Core.Agencies;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.BasicData;
using ThemePark.Infrastructure.Application;

namespace ThemePark.ApplicationDto.AgentTicket
{
    /// <summary>
    /// 团体可售票类Dto
    /// </summary>
    [AutoMap(typeof(GroupTypeTicketClass))]
    public class GroupTypeTicketClassDto : FullAuditedEntityDto
    {
        /// <summary>
        /// 公园Id
        /// </summary>
        public int ParkId { get; set; }

        /// <summary>
        /// 团体ID
        /// </summary>
        public int GroupTypeId { get; set; }

        /// <summary>
        /// 基础票类ID
        /// </summary>
        public int TicketClassId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public GroupTypeDto GroupType { get; set; }

        public TicketClassDto TicketClass { get; set; }
    }

    /// <summary>
    /// 团体可售票类页面显示Dto
    /// </summary>
    [AutoMap(typeof(GroupTypeTicketClass))]
    public class GroupTypeTicketClassList
    {
        /// <summary>
        /// 公园Id
        /// </summary>
        public int ParkId { get; set; }

        /// <summary>
        /// 公园名称
        /// </summary>
        [MapFrom(nameof(GroupTypeTicketClass.Park),nameof(Park.ParkName))]
        public string ParkName { get; set; }

        /// <summary>
        /// 团体ID
        /// </summary>
        public int GroupTypeId { get; set; }

        /// <summary>
        /// 带队类型名称
        /// </summary>    
        [MapFrom(nameof(GroupTypeTicketClass.GroupType), nameof(GroupType.TypeName))]
        public string GroupTypeName { get; set; }

        /// <summary>
        /// 基础票类列表
        /// </summary>
        public IList<string> TicketClasses { get; set; }

}

    /// <summary>
    /// 为页面显示（组装Dto）
    /// </summary>
    public class GrouptTypeTicketClassAssembleDto : FullAuditedEntityDto
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public GrouptTypeTicketClassAssembleDto()
        {
            TicketClasses = new List<TicketClassDto>();
        }
        /// <summary>
        /// 公园Id
        /// </summary>
        public int ParkId { get; set; }

        /// <summary>
        /// 公园名称
        /// </summary>
        public string ParkName { get; set; }

        /// <summary>
        /// 团队类型Id
        /// </summary>
        public int GroupTypeId { get; set; }

        /// <summary>
        /// 团队名称
        /// </summary>
        public string GroupTypeName { get; set; }

        /// <summary>
        /// 票类列表
        /// </summary>
        public List<TicketClassDto> TicketClasses { get; set; }
        
    }
}
