using System.ComponentModel.DataAnnotations;

namespace ThemePark.Infrastructure.Enumeration
{
    /// <summary>
    /// 票的分类
    /// </summary>
    public enum TicketCategory
    {
        /// <summary>
        /// 散客
        /// </summary>
        [Display(Name = "散客票")]
        NonGroupTicket = 1,

        /// <summary>
        /// 团体
        /// </summary>
        [Display(Name = "团体票")]
        GroupTicket = 2,

        /// <summary>
        /// 订单票
        /// </summary>
        [Display(Name = "订单")]
        Order = 3,

        /// <summary>
        /// 他园票
        /// </summary>
        [Display(Name = "他园票")]
        OtherNonGroupTicket = 4,

        /// <summary>
        /// 网络票
        /// </summary>
        [Display(Name = "网络票")]
        ToTicket = 5,

        /// <summary>
        /// 入园单
        /// </summary>
        [Display(Name = "入园单")]
        InParkBill = 6
    }
}
