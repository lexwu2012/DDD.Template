using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;
using ThemePark.ApplicationDto.Users;
using ThemePark.Core.Agencies;

namespace ThemePark.ApplicationDto.Agencies
{
    /// <summary>
    /// 新增旅行社用户
    /// </summary>
    [AutoMapTo(typeof(AgencyUser))]
    public class AddAgencyUserInput
    {
        #region Properties

        /// <summary>
        /// 代理商Id
        /// </summary>
        [Required]
        public int AgencyId { get; set; }

        /// <summary>
        /// 许可主机IP
        /// </summary>
        public string HostIPs { get; set; }

        /// <summary>
        /// Gets or sets the we chart no.
        /// </summary>
        /// <value>The we chart no.</value>
        public string WeChatNo { get; set; }

        /// <summary>
        /// User
        /// </summary>
        public AddUserInput User { get; set; }

        #endregion Properties
    }
}