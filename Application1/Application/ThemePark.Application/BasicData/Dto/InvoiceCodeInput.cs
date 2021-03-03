using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.BasicData.Dto
{
    /// <summary>
    /// 发票代码
    /// </summary>
    [AutoMapFrom(typeof(InvoiceCode))]
    public class InvoiceCodeInput
    {
        /// <summary>
        /// 发票代码
        /// </summary>
        [Required, StringLength(20)]
        public string Code { get; set; }

        /// <summary>
        /// 发票单位
        /// </summary>
        [StringLength(100)]
        public string Company { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 是否定期上传
        /// </summary>
        public bool IsUpload { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }

        /// <summary>
        /// 发票号是否递增，否的话递减
        /// </summary>
        public bool InvoiceNumIsIncrease { get; set; }

        /// <summary>
        /// 使用模块
        /// </summary>
        public SaleModuelType SaleModuel { get; set; }
    }
}
