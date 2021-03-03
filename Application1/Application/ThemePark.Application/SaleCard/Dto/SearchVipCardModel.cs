using ThemePark.Core.CardManage;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.SaleCard.Dto
{
     /// <summary>
    /// 年卡信息管理查询条件
   /// </summary>
    public class SearchVipCardModel : PageSortInfo
    {
        /// <summary>
        /// 卡内码
        /// </summary>
        public string IcNo { get; set; }

        ///// <summary>
        ///// 电子年卡编号
        ///// </summary>
        //public string ECardID { get; set; }

        /// <summary>
        /// 状态
        /// </summary>    
        public VipCardStateType? State { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// 年卡类型ID
        /// </summary>
        public int?  TicketClassId { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 激活时间
        /// </summary>    
        public System.DateTime? ActiveTime { get; set; }

        /// <summary>
        /// 开始有效日期
        /// </summary>    
        public System.DateTime? ValidDateBegin { get; set; }

        /// <summary>
        /// 结束有效日期
        /// </summary>    
        public System.DateTime? ValidDateEnd { get; set; }
    }
}
