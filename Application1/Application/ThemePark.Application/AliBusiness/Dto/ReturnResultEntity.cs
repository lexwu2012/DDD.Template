using System.ComponentModel.DataAnnotations;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.AliBusiness.Dto
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class ReturnResultEntity<TData>
    {
        /// <summary>
        /// 返回的数据
        /// </summary>
        public TData Data { get; set; }

        /// <summary>
        /// 返回结果状态
        /// </summary>
        public ResultState ResultStatus { get; set; }

        /// <summary>
        /// 返回消息
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// 状态码
    /// </summary>
    public enum ResultState
    {
        /// <summary>
        /// 操作成功
        ///</summary>
        [Display(Name = "操作成功", GroupName = Result.SuccessCode)]
        Ok = 0,

        /// <summary>
        /// 操作失败
        ///</summary>
        [Display(Name = "操作失败")]
        Fail = 1,

        /// <summary>
        /// 服务数据异常
        ///</summary>
        [Display(Name = "服务数据异常")]
        ServerError = 2,

        /// <summary>
        /// 身份验证失败
        ///</summary>
        [Display(Name = "身份验证失败")]
        Unauthorized = 3,

        /// <summary>
        /// 参数验证失败
        /// </summary>
        [Display(Name = "参数验证失败")]
        InvalidData = 4,

        /// <summary>
        /// 必传参数欠缺
        /// </summary>
        [Display(Name = "必传参数欠缺")]
        RequireData = 5,

        /// <summary>
        /// Token 失效
        /// </summary>
        [Display(Name = "Token 失效")]
        InvalidToken = 6,

        /// <summary>
        /// 订单不存在
        /// </summary>
        [Display(Name = "订单不存在")]
        OrderidNoExists = 7,

        /// <summary>
        /// 当地服务器不提供查询
        /// </summary>
        [Display(Name = "当地服务器不提供查询")]
        ParkServiceNotProvide = 8,

        /// <summary>
        /// 当地服务器调用报错
        /// </summary>
        [Display(Name = "当地服务器调用报错")]
        ParkServiceError = 9,

        /// <summary>
        /// 用户ip不在允许范围
        /// </summary>
        [Display(Name = "用户ip不在允许范围")]
        UserIpNotAllow = 10,

        /// <summary>
        /// 身份证验证成功
        /// </summary>
        [Display(Name = "身份证验证成功", GroupName = Result.SuccessCode)]
        IdnumCheckSuccess = 11,

        /// <summary>
        /// 预订成功，等待支付确认
        /// </summary>
        [Display(Name = "预订成功，等待支付确认", GroupName = Result.SuccessCode)]
        OrderBookSuccess = 99,

        /// <summary>
        /// 订票成功
        /// </summary>
        [Display(Name = "订票成功", GroupName = Result.SuccessCode)]
        OrderSuccess = 100,

        /// <summary>
        /// 订单票数据异常
        /// </summary>
        [Display(Name = "订单票数据异常")]
        TicketDataError = 101,

        /// <summary>
        /// 订单日期有误
        /// </summary>
        [Display(Name = "订单日期有误")]
        OrderPlanDataError = 102,

        /// <summary>
        /// 公园不存在
        /// </summary>
        [Display(Name = "公园不存在")]
        ParkNoExist = 103,

        /// <summary>
        /// 订单已经存在
        /// </summary>
        [Display(Name = "订单已经存在")]
        OrderAlreadyExist = 104,

        /// <summary>
        /// 取票码已经被使用
        /// </summary>
        [Display(Name = "取票码已经被使用")]
        PrepaymentLimit = 105,

        /// <summary>
        /// 订单中身份证订票数量超出限制
        /// </summary>
        [Display(Name = "订单中身份证订票数量超出限制")]
        OrderIdnumUseLimit = 106,

        /// <summary>
        /// 订单中票类无效
        /// </summary>
        [Display(Name = "订单中票类无效")]
        OrderTicketTypeIdError = 107,

        /// <summary>
        /// 订单中身份证已订过票
        /// </summary>
        [Display(Name = "订单中身份证已订过票")]
        OrderIdnumUseAlready = 108,

        /// <summary>
        /// 身份证格式错误
        /// </summary>
        [Display(Name = "身份证格式错误")]
        IdnumError = 109,

        /// <summary>
        /// 订单票价错误
        /// </summary>
        [Display(Name = "订单票价错误")]
        OrderTicketPriceError = 110,

        /// <summary>
        /// 订单总价错误
        /// </summary>
        [Display(Name = "订单总价错误")]
        OrderTotalPriceError = 111,

        /// <summary>
        /// 订单中只能有一张身份证
        /// </summary>
        [Display(Name = "订单中只能有一张身份证")]
        OrderIdnumLimit = 112,

        /// <summary>
        /// 订单数据不符合要求
        /// </summary>
        [Display(Name = "订单数据不符合要求")]
        OrderTicketDataError = 113,

        /// <summary>
        /// 订单已支付
        /// </summary>
        [Display(Name = "订单已支付")]
        OrderTicketPayed = 114,

        /// <summary>
        /// 订单已取消
        /// </summary>
        [Display(Name = "订单已取消")]
        OrderAlreadyCancel = 201,

        /// <summary>
        /// 订单取消失败
        /// </summary>
        [Display(Name = "订单取消失败")]
        OrderCancelFail = 202,

        /// <summary>
        /// 未支付订单不允许退票
        /// </summary>
        [Display(Name = "未支付订单不允许退票")]
        OrderCancelFailForNoPay = 203,

        /// <summary>
        /// 订单取消成功
        /// </summary>
        [Display(Name = "订单取消成功")]
        OrderCancelSuccess = 204,

        /// <summary>
        /// 订单取消失败，订单号不存在
        /// </summary>
        [Display(Name = "订单取消失败，订单号不存在")]
        OrderCancelFailOrderidNoExist = 205,

        /// <summary>
        /// 订单取消失败，订单号不存在
        /// </summary>
        [Display(Name = "订单取消失败，订单号不存在")]
        OrderCancelFailParkOrderidNoExist = 206,

        /// <summary>
        /// 订单取消失败，订单已使用
        /// </summary>
        [Display(Name = "订单取消失败，订单已使用")]
        OrderCancelFailAlreadyUsed = 207,

        /// <summary>
        /// 订单支付成功
        /// </summary>
        [Display(Name = "订单支付确认成功")]
        OrderPaySuccess = 208,

        /// <summary>
        /// 订单取消失败，订单已使用
        /// </summary>
        [Display(Name = "订单取消失败，订单已冻结")]
        OrderCancelFailAlreadyFrozen = 209,

        /// <summary>
        /// 订单已经取消，不允许修改
        /// </summary>
        [Display(Name = "订单已经取消，不允许修改")]
        OrderUpdateFailAlreadyCancel = 301,

        /// <summary>
        /// 订单已经使用，不允许修改
        /// </summary>
        [Display(Name = "订单已经使用，不允许修改")]
        OrderUpdateFailAlreadyUsed = 302,

        /// <summary>
        /// 库存已售馨
        /// </summary>
        [Display(Name = "库存已售馨")]
        TicketSaleOut = 303,

    }
}
