using Abp.AutoMapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 团体信息输入（每个旅行社订单都会生成一个团体信息，这个团体信息就是里面有什么导游和司机等信息）
    /// </summary>
    [AutoMapTo(typeof(GroupInfo))]
    public class GroupInfoInput
    {
        /// <summary>
        /// cotr
        /// </summary>
        public GroupInfoInput()
        {
            GuideIdList = new List<int>();
            DriverIdList = new List<int>();
        }

        /// <summary>
        /// 旅游团体名称（用代理商名称命名）
        /// </summary>
        [Required]
        [StringLength(50)]
        public string GroupInfoName { get; set; }
        
        /// <summary>
        /// 导游编号<example>11,12,13</example>>
        /// </summary>        
        public IList<int> GuideIdList { get; set; }

        /// <summary>
        /// 司陪<example>11,12,13</example>>
        /// </summary>
        public IList<int> DriverIdList { get; set; }
        
        /// <summary>
        /// 车牌号码
        /// </summary>
        public string LicensePlateNumber { get; set; }

        ///// <summary>
        ///// 团队成员
        ///// </summary>
        //public IList<GroupMemberInput> GroupMembers { get; set; }               
    }
}
