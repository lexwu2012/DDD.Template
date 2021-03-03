using System.ComponentModel.DataAnnotations;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.SaleCard.Dto
{
    /// <summary>
    /// 客户信息
    /// </summary>
    public  class CustomerInput
    {
        /// <summary>
        /// Id
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// 顾客姓名
        /// </summary>
        [StringLength(20)]
        public string CustomName { get; set; }

        /// <summary>
        /// 指纹模板1
        /// </summary>
        public string Fp1 { get; set; }

        /// <summary>
        /// 指纹图片
        /// </summary>
        public string FpImage1 { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public GenderType Gender { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Mail { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 相片
        /// </summary>
        public string Photo { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [StringLength(100)]
        public string Address { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public string Birthday { get; set; }

    }
}
