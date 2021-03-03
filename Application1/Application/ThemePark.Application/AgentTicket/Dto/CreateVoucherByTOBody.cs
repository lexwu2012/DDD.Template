using Abp.AutoMapper;
using ThemePark.Core.AgentTicket;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 由TOBody创建凭证TOVoucher
    /// </summary>
    [AutoMapFrom(typeof(TOBody))]
    public class CreateVoucherByTOBody
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 公园编号
        /// </summary>    
        public int ParkId { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>    
        public int? CustId { get; set; }

        /// <summary>
        /// 序号
        /// </summary>    
        public int Seq { get; set; }
    }
}
