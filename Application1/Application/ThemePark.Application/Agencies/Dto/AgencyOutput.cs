using Abp.AutoMapper;
using ThemePark.ApplicationDto.BasicData;
using ThemePark.Core.Agencies;

namespace ThemePark.Application.Agencies.Dto
{
    [AutoMapFrom(typeof(Agency))]
    public class AgencyOutput
    {
        /// <summary>
        /// 旅行社ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 代理商名称
        /// </summary>    
        public string AgencyName { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>    
        public string Email { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>    
        public string Tel { get; set; }
        /// <summary>
        /// 联系手机
        /// </summary>    
        public string Mobile { get; set; }

        /// <summary>
        /// 备注
        /// </summary>    
        public string Remark { get; set; }

        /// <summary>
        /// 旅行社所在地址
        /// </summary>
        public AddressDto Address { get; set; }
    }
}
