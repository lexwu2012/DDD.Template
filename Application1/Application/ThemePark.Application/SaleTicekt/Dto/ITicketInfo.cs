
namespace ThemePark.Application.SaleTicekt.Dto
{
    /// <summary>
    /// 购票信息
    /// </summary>
    public interface ITicketInfo
    {
        /// <summary>
        /// 数量
        /// </summary>
        int Qty { get; set; }

        /// <summary>
        /// 票类编号
        /// </summary>
        int TicketClassId { get;}
    }
}