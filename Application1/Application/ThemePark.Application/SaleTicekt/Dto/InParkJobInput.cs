using Abp.AutoMapper;
using ThemePark.Core.ParkSale;

namespace ThemePark.Application.SaleTicket.Dto
{
    /// <summary>
    /// 后台作业需要更新的参数
    /// </summary>
    [AutoMap(typeof(NonGroupTicket))]
    public class InParkJobInput
    {
        /// <summary>
        /// 
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int InparkCounts { get; set; }

        /// <summary>
        /// 
        /// </summary>
        
        public TicketSaleStatus State { get; set; }
    }
}
