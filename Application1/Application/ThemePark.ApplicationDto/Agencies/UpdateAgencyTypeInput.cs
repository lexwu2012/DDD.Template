using Abp.AutoMapper;
using System.Collections.Generic;
using ThemePark.Core.Agencies;

namespace ThemePark.ApplicationDto.Agencies
{
    /// <summary>
    ///  
    /// </summary>
    [AutoMapTo(typeof(Core.Agencies.AgencyType))]
    public class UpdateAgencyTypeInput
    {
        public int Id { get; set; }

        /// <summary>
        /// 代理商名称
        /// </summary>
        public string AgencyTypeName { get; set; }
        /// <summary>
        /// 备注
        /// </summary>    
        public string Remark { get; set; }

        /// <summary>
        /// 默认排序
        /// </summary>
        public int SerialNumber { get; set; }

        /// <summary>
        /// 默认代理商类型
        /// </summary>
        public DefaultAgencyType DefaultAgencyType { get; set; }

        /// <summary>
        /// 代理商团体类型
        /// </summary>
        public List<AddParkAgencyTypeGroupTypeInput> AgencyTypeGroupTypeInputs { get; set; }

        public UpdateAgencyTypeInput()
        {
            AgencyTypeGroupTypeInputs = new List<AddParkAgencyTypeGroupTypeInput>();
        }
    }
}
