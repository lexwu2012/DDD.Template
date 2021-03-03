using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ThemePark.Core.ParkSale;

namespace ThemePark.Application.SaleTicekt.Dto
{
    /// <summary>
    /// 作废票记录dto
    /// </summary>
    [AutoMap(typeof(ExcessFareInvalid))]
    public class ExcessFareInvalidDto : FullAuditedEntityDto
    {
        /// <summary>
        /// 条码号
        /// </summary>    
        public string Barcode { get; set; }
        /// <summary>
        /// 交易号
        /// </summary>    
        public string Tradeno { get; set; }
        /// <summary>
        /// 终端号
        /// </summary>    
        public int TerminalId { get; set; }
        /// <summary>
        /// 备注
        /// </summary>    
        public string Remark { get; set; }
    }
}
