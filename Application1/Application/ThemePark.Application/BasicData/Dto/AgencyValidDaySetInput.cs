using System.Collections.Generic;
using Abp.AutoMapper;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.BasicData.Dto
{
    [AutoMapTo(typeof(AgencyPrintSet))]
    public class AgencyValidDaysSetInput
    {
        /// <summary>
        /// 待修改代理商
        /// </summary>
        public List<AgencyValidDayInfo> AgencyInfos { get; set; }

        /// <summary>
        /// 有效期
        /// </summary>
        public int? ValidDays { get; set; }


    }

    /// <summary>
    /// 需要设置的代理商
    /// </summary>
    public class AgencyValidDayInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 代理商ID
        /// </summary>
        public int AgencyId { get; set; }

        /// <summary>
        /// 代理商类型ID
        /// </summary>
        public int AgencyTypeId { get; set; }
    }
}
