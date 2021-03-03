using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ThemePark.Core.Agencies;

namespace ThemePark.Application.Agencies.Dto
{
    /// <summary>
    /// 
    /// </summary>
    [AutoMap(typeof(Account))]
    public class AccountPreDto : FullAuditedEntityDto
    {
        /// <summary>
        /// 单位名称
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 余额
        /// </summary>    
        public decimal Balance { get; set; }

        /// <summary>
        /// 报警金额
        /// </summary>
        public decimal AlarmBalance { get; set; }

        /// <summary>
        /// 最低金额
        /// </summary>
        public decimal LeastBalance { get; set; }

        /// <summary>
        /// 方特预警接收手机号
        /// </summary>
        public string FtPeoplePhone { get; set; }

        /// <summary>
        /// 旅行社预警接收手机号
        /// </summary>
        public string AgencyPeoplePhone { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

    }
}
