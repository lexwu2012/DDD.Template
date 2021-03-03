using System;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public class GetAgencySaleTicketByWindowInput
    {
        /// <summary>
        /// 代理商ID
        /// </summary>
        public int AgencyId { get; set; }

        /// <summary>
        /// 公园ID
        /// </summary>
        public int ParkId { get; set; }

        /// <summary>
        /// 团体类型ID
        /// </summary>
        public int GroupTypeId { get; set; }


        /// <summary>
        /// 预订日期
        /// </summary>
        public DateTime? Plandate { get; set; }
        
    }
}
