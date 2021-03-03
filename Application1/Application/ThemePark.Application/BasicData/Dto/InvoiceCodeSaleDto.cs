using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.BasicData.Dto
{
    /// <summary>
    /// 发票代码
    /// </summary>
    [AutoMap(typeof(InvoiceCode))]
    public class InvoiceCodeSaleDto
    {
        /// <summary>
        /// 发票代码ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 发票代码
        /// </summary>
        [Required, StringLength(20)]
        public string Code { get; set; }
    }
}
