using System;
using Abp.AutoMapper;
using ThemePark.Core.AgentTicket;
using ThemePark.Core.BasicTicketType;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 电商可售票类数据
    /// </summary>
    [AutoMapFrom(typeof(AgencySaleTicketClass))]
    public class ApiAgencySaleTicketTypeDto
    {

        /// <summary>
        /// 公园编号
        /// </summary>
        public short ParkCode { get; set; }

        /// <summary>
        /// 公园名称
        /// </summary>
        public string ParkName { get; set; }

        /// <summary>
        /// 票类代码
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 基础票种代码
        /// </summary>
        [MapFrom(nameof(AgencySaleTicketClass.ParkSaleTicketClass),nameof(ParkSaleTicketClass.TicketClass),nameof(TicketClass.TicketTypeId))]
        public string TicketTypeId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime LastModificationTime { get; set; }

        /// <summary>
        /// 票类名称
        /// </summary>
        public string AgencySaleTicketClassName { get; set; }

        /// <summary>
        /// 结算价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 结算价格
        /// </summary>
        public decimal StandPrice { get; set; }

        /// <summary>
        /// 开始销售日期
        /// </summary>
        public DateTime SaleStartDate { get; set; }

        /// <summary>
        /// 结束销售日期
        /// </summary>
        public DateTime SaleEndDate { get; set; }

    }
}
