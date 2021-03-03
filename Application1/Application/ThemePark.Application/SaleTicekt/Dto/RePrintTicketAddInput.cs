using System.ComponentModel.DataAnnotations;
using ThemePark.Application.SaleTicekt.Interfaces;

namespace ThemePark.Application.SaleTicekt.Dto
{
    /// <summary>
    /// 重打印新增
    /// </summary>
    public class RePrintTicketAddInput
    {        
        /// <summary>
        /// 条形码
        /// </summary>
        [Required]
        public string BarCode { get; set; }                
    }
}
