using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.ComponentModel.DataAnnotations;
using ThemePark.Core.ParkSale;

namespace ThemePark.Application.SaleTicekt.Dto
{
    /// <summary>
    /// 补票dto（主键是barCode）
    /// </summary>
    [AutoMap(typeof(ExcessFare))]
    public class FareAdjustmentDto : FullAuditedEntityDto<string>
    {
        /// <summary>
        /// 公园编号
        /// </summary>    
        public int ParkId { get; set; }

        /// <summary>
        /// 公园名称
        /// </summary>
        //[MapFrom(nameof(ExcessFare.Park),nameof(Park.ParkName))]
        //public string ParkName { get; set; }

        /// <summary>
        /// 交易号
        /// </summary>    
        public string TradeInfoId { get; set; }

        /// <summary>
        /// 面额
        /// </summary>    
        public decimal Denomination { get; set; }

        /// <summary>
        /// 张数
        /// </summary>    
        public int Qty { get; set; }

        /// <summary>
        /// 补票金额
        /// </summary>    
        public decimal Amount { get; set; }

        /// <summary>
        /// 状态：1 有效 2 已使用 3 作废
        /// </summary>    
        public TicketSaleStatus State { get; set; }

        /// <summary>
        /// 终端号
        /// </summary>    
        public int TerminalId { get; set; }

        /// <summary>
        /// 有效期限
        /// </summary>    
        public DateTime ValidEndDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>    
        [StringLength(50)]
        public string Remark { get; set; }
    }
}
