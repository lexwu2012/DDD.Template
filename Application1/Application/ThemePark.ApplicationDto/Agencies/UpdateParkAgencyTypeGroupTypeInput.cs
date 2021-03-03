using Abp.AutoMapper;
using ThemePark.Core.Agencies;

namespace ThemePark.ApplicationDto.Agencies
{
    /// <summary>
    /// 更改输入
    /// </summary>
    [AutoMapTo(typeof (ParkAgencyTypeGroupType))]
    public class UpdateParkAgencyTypeGroupTypeInput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 代理商票类规则ID
        /// </summary>
        public int AgencyRuleId { get; set; }

    }
}
