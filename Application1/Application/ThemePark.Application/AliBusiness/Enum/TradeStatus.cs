
namespace ThemePark.Application.AliBusiness.Enum
{
    /// <summary>
    /// 交易状态
    /// 可选值: 
    /// * TRADE_NO_CREATE_PAY(没有创建支付宝交易) 
    /// * WAIT_BUYER_PAY(等待买家付款) 
    /// * SELLER_CONSIGNED_PART(卖家部分发货) 
    /// * WAIT_SELLER_SEND_GOODS(等待卖家发货, 即:买家已付款) 
    /// * WAIT_BUYER_CONFIRM_GOODS(等待买家确认收货, 即:卖家已发货) 
    /// * TRADE_BUYER_SIGNED(买家已签收, 货到付款专用) 
    /// * TRADE_FINISHED(交易成功) 
    /// * TRADE_CLOSED(付款以后用户退款成功，交易自动关闭) 
    /// * TRADE_CLOSED_BY_TAOBAO(付款以前，卖家或买家主动关闭交易) 
    /// * PAY_PENDING(国际信用卡支付付款确认中) 
    /// * WAIT_PRE_AUTH_CONFIRM(0元购合约中)
    /// </summary>
    public enum TradeStatus
    {        
        /// <summary>
        /// 没有创建支付宝交易
        /// </summary>
        TRADE_NO_CREATE_PAY,
        /// <summary>
        /// 等待买家付款
        /// </summary>
        WAIT_BUYER_PAY,
        /// <summary>
        /// 卖家部分发货
        /// </summary>
        SELLER_CONSIGNED_PART,
        /// <summary>
        /// 等待卖家发货, 即:买家已付款
        /// </summary>
        WAIT_SELLER_SEND_GOODS,
        /// <summary>
        /// 等待买家确认收货, 即:卖家已发货
        /// </summary>
        WAIT_BUYER_CONFIRM_GOODS,
        /// <summary>
        /// 买家已签收, 货到付款专用
        /// </summary>
        TRADE_BUYER_SIGNED,
        /// <summary>
        /// 交易成功
        /// </summary>
        TRADE_FINISHED,
        /// <summary>
        /// 付款以后用户退款成功，交易自动关闭
        /// </summary>
        TRADE_CLOSED,
        /// <summary>
        /// 付款以前，卖家或买家主动关闭交易
        /// </summary>
        TRADE_CLOSED_BY_TAOBAO,
        /// <summary>
        /// 国际信用卡支付付款确认中
        /// </summary>
        PAY_PENDING,
        /// <summary>
        /// 0元购合约中
        /// </summary>
        WAIT_PRE_AUTH_CONFIRM
    }
}
