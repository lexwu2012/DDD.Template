using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ThemePark.Core.Agencies;

namespace ThemePark.Application.Agencies.Dto
{
    /// <summary>
    /// 代理商账户dto
    /// </summary>
    [AutoMap(typeof(Agency))]
    public class AgencyPreAccountDto : FullAuditedEntityDto
    {
        /// <summary>
        /// 代理商账户Id
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// 代理商名称
        /// </summary>
        public string AgencyName { get; set; }

        /// <summary>
        /// 代理商账户
        /// </summary>
        /// <value>The account.</value>
        public virtual AccountPreDto Account { get; set; }



    }
}
