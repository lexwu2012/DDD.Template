using Abp.AutoMapper;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 导游输入
    /// </summary>
    [AutoMap(typeof(GuideCustomer))]
    public class GuideCustInput 
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 代理商Id
        /// </summary>
        public int AgencyId { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Required]
        [DisplayName("导游姓名")]
        [StringLength(20)]
        public string Name { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [Required]
        [DisplayName("手机号")]
        public string Phone { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        [Required]
        [DisplayName("身份证号")]
        [StringLength(18)]
        public string Idcard { get; set; }
    }
}
