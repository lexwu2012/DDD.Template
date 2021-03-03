using System.ComponentModel;
using Abp.AutoMapper;
using ThemePark.Core.Agencies;

namespace ThemePark.ApplicationDto.Agencies
{
    [AutoMapTo(typeof(ParkAgencyTypeGroupType))]
    public class AddParkAgencyTypeGroupTypeInput 
    {
        /// <summary>
        /// 代理商ID
        /// </summary>
        public int AgencyTypeId { get; set; }

        /// <summary>
        /// 团体类型ID
        /// </summary>
        public int GroupTypeId { get; set; }

        /// <summary>
        /// 代理商票类规则ID
        /// </summary>
        [DisplayName("代理商规则")]
        public int AgencyRuleId { get; set; }

        /// <summary>
        /// 公园ID
        /// </summary>
        [DisplayName("公园")]
        public int ParkId { get; set; }
    }
}
