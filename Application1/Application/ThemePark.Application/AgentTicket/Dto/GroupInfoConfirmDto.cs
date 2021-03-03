using Abp.AutoMapper;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 团体信息订单确认dto
    /// </summary>
    [AutoMapTo(typeof(GroupInfo))]
    public class GroupInfoConfirmDto
    {
        /// <summary>
        /// 导游编号<example>11,12,13</example>>
        /// </summary>
        public string GuideIds { get; set; }

        /// <summary>
        /// 司陪<example>11,12,13</example>>
        /// </summary>
        public string DriverIds { get; set; }

        /// <summary>
        /// 驾驶证
        /// </summary>
        public string LicensePlateNumber { get; set; }
    }
}
