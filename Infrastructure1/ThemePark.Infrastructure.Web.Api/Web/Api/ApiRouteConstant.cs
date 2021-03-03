
namespace ThemePark.Infrastructure.Web.Api
{
    public class ApiRouteConstant
    {
        /// <summary>
        /// root
        /// </summary>
        public const string ApiPrefix = "Api/";

        #region ParkApiModule

        #region TicketBusinessApiController
        /// <summary>
        /// 票业务
        /// </summary>
        public const string TicketBusinessRoute = "TicketBusiness";

        /// <summary>
        /// 
        /// </summary>
        public const string VipCard = "VipCard";

        /// <summary>
        /// 
        /// </summary>
        public const string GetECardPhoto = "GetECardPhoto";

        /// <summary>
        /// 
        /// </summary>
        public const string GetECardDetial = "GetECardDetial";

        /// <summary>
        /// 
        /// </summary>
        public const string GetECardDetialNoPhoto = "GetECardDetialNoPhoto";

        /// <summary>
        /// 
        /// </summary>
        public const string MultiTicketCancelRoute = "MultiTicketCancel";

        /// <summary>
        /// 
        /// </summary>
        public const string OtherNonGroupTicketRefundRoute = "OtherNonGroupTicketRefundAsync";


        #endregion

        #region OrderBusinessApiController
        /// <summary>
        /// 订单业务
        /// </summary>
        public const string OrderBusinessRoute = "OrderBusiness";

        /// <summary>
        /// 获取订单详情
        /// </summary>
        public const string FetchOrderDetailRoute = "FetchOrderDetailAsync";

        /// <summary>
        /// 冻结订单API路径
        /// </summary>
        public const string FreezeOrderRoute = "FreezeOrderAsync";

        /// <summary>
        /// 取消订单api路径
        /// </summary>
        public const string OrderCancelConfirmRoute = "OrderCancelConfirmAsync";

        /// <summary>
        /// Job测试
        /// </summary>
        public const string JobTest = "JobTest";

        #endregion

        #region OTAOrderApiController
        /// <summary>
        /// ota订单业务api路径
        /// </summary>
        public const string OtaBusinessRoute = "OtaOrder";

        /// <summary>
        /// ota预付款扣款API路径
        /// </summary>
        public const string DeductMoneyRoute = "DeductMoneyAsync";

        #endregion

        #endregion

        #region OTAApiModule

        #endregion
    }
}
