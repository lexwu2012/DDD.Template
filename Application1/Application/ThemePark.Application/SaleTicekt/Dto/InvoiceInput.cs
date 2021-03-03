using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using ThemePark.Core.ParkSale;

namespace ThemePark.Application.SaleTicekt.Dto
{
    /// <summary>
    /// 客户端发票相关信息
    /// </summary>
    [AutoMapTo(typeof(Invoice))]
    public class InvoiceInput
    {
        /// <summary>
        /// 发票号
        /// </summary> 
        [StringLength(18)]
        [DisplayName("发票号")]
        public string InvoiceNo { get; set; }

        /// <summary>
        /// 发票代码
        /// </summary>
        [DisplayName("发票代码")]
        public string InvoiceCode { get; set; }
    }
}
