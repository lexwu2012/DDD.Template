using System.Collections.Generic;
using Abp.AutoMapper;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 团体信息更改输入
    /// </summary>
    [AutoMapTo(typeof(GroupInfo))]
    public class GroupInfoEditInput
    {
        public GroupInfoEditInput()
        {
            //GuideIdList = new List<int>();
            //DriverIdList = new List<int>();
        }

        ///// <summary>
        ///// 导游编号<example>11,12,13</example>>
        ///// </summary>        
        //public IList<int> GuideIdList { get; set; }

        ///// <summary>
        ///// 司陪<example>11,12,13</example>>
        ///// </summary>
        //public IList<int> DriverIdList { get; set; }

        /// <summary>
        /// 导游编号<example>11,12,13</example>>
        /// </summary>
        public string GuideIds { get; set; }

        /// <summary>
        /// 司陪<example>11,12,13</example>>
        /// </summary>
        public string DriverIds { get; set; }

        /// <summary>
        /// 车牌号码
        /// </summary>
        public string LicensePlateNumber { get; set; }
    }
}
