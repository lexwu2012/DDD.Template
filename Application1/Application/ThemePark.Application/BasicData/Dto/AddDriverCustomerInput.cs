using Abp.AutoMapper;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.BasicData.Dto
{
    /// <summary>
    /// 司机输入
    /// </summary>
    [AutoMap(typeof(DriverCustomer))]
    public class DriverCustomerInput
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
        [DisplayName("司陪姓名")]
        [StringLength(20)]
        public string Name { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        [Required]
        [StringLength(18)]
        [DisplayName("身份证号")]
        public string Idcard { get; set; }
    }
}
