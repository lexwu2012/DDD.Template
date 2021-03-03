using Abp.AutoMapper;
using ThemePark.Core.Agencies;

namespace ThemePark.Application.Agencies.Dto
{
    /// <summary>
    /// 默认代理商类型
    /// </summary>
    [AutoMap(typeof(AgencyType))]
    public class AgencyTypeWithDefaultType
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 代理商类型名称
        /// </summary>    
        public string AgencyTypeName { get; set; }

        /// <summary>
        /// 默认代理商类型
        /// </summary>
        public DefaultAgencyType DefaultAgencyType { get; set; }
    }
}
