using System.ComponentModel.DataAnnotations;

namespace ThemePark.Infrastructure.Application
{
    /// <summary>
    /// Api请求状态码
    /// </summary>
    public enum ResultCode
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
        ServerError = 10,

        /// <summary>
        /// 未登录
        ///</summary>
        [Display(Name = "未登录")]
        Unauthorized = 20,

        /// <summary>
        /// 未授权
        /// </summary>
        [Display(Name = "未授权")]
        Forbidden = 21,

        /// <summary>
        /// Token 失效
        /// </summary>
        [Display(Name = "Token 失效")]
        InvalidToken = 22,

        /// <summary>
        /// 密码验证失败
        /// </summary>
        [Display(Name = "密码验证失败")]
        SpaFailed = 23,

        /// <summary>
        /// 错误的新密码
        /// </summary>
        [Display(Name = "错误的新密码")]
        WrongNewPassword = 24,

        /// <summary>
        /// 签名验证失败
        /// </summary>
        [Display(Name = "签名验证失败")]
        InvalidSign = 402,

        /// <summary>
        /// 参数验证失败
        /// </summary>
        [Display(Name = "参数验证失败")]
        InvalidData = 403,

        /// <summary>
        /// 没有此条记录
        ///</summary>
        [Display(Name = "没有此条记录")]
        NoRecord = 404,

        /// <summary>
        /// 重复记录
        /// </summary>
        [Display(Name = "已有记录，请勿重复操作")]
        DuplicateRecord = 405,

        /// <summary>
        /// 缺失基础数据
        /// </summary>
        [Display(Name = "缺失基础数据")]
        MissEssentialData = 406,

        /// <summary>
        /// 金额验证失败
        /// </summary>
        [Display(Name = "金额验证失败")]
        InvalidAmount = 407,

        /// <summary>
        /// 缺少对应票类
        /// </summary>
        [Display(Name = "缺少对应票类")]
        MissTicketType = 408,

        /// <summary>
        /// 支付失败
        /// </summary>
        [Display(Name = "支付失败")]
        PayFail = 500,

        /// <summary>
        /// 写入出票记录失败
        /// </summary>
        [Display(Name = "写入出票记录失败")]
        WriteTicketRecordFail = 501,

        /// <summary>
        /// 写入年卡凭证记录失败
        /// </summary>
        [Display(Name = "写入年卡凭证记录失败")]
        WriteVoucherRecordFail = 502,


        /// <summary>
        /// 存在重复发票号或发票号为负数
        /// </summary>
        [Display(Name = "存在重复发票号或发票号为负数")]
        DuplicateInvoiceRecord = 503,

        /// <summary>
        /// 预付款余额已经低于最低限制金额
        /// </summary>
        [Display(Name = "预付款余额已经低于最低限制金额")]
        InsufficientBalance = 504,


        #region 自助售票机接口状态码 1000~2000

        /// <summary>
        /// 订单不存在或已过期
        /// </summary>
        [Display(Name = "订单不存在或已过期")]
        VendorOrderNoExists = 1000,

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "订单已取票")]
        VendorOrderConsumed = 1001,


        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "余票不足")]
        VendorTicketNotEnough = 1002,



        #endregion

        #region 电商平台接口状态码 10000~19999



        /// <summary>
        /// 订单不存在
        /// </summary>
        [Display(Name = "订单不存在")]
        OrderidNoExists = 7 + 10000,

        /// <summary>
        /// 当地服务器不提供查询
        /// </summary>
        [Display(Name = "当地服务器不提供查询")]
        ParkServiceNotProvide = 8 + 10000,

        /// <summary>
        /// 当地服务器调用报错
        /// </summary>
        [Display(Name = "当地服务器调用报错")]
        ParkServiceError = 9 + 10000,

        /// <summary>
        /// 用户ip不在允许范围
        /// </summary>
        [Display(Name = "用户ip不在允许范围")]
        UserIpNotAllow = 10 + 10000,

        /// <summary>
        /// 身份证验证成功
        /// </summary>
        [Display(Name = "身份证验证成功", GroupName = Result.SuccessCode)]
        IdnumCheckSuccess = 11 + 10000,

        /// <summary>
        /// 预订成功，等待支付确认
        /// </summary>
        [Display(Name = "预订成功，等待支付确认", GroupName = Result.SuccessCode)]
        OrderBookSuccess = 99 + 10000,

        /// <summary>
        /// 订票成功
        /// </summary>
        [Display(Name = "订票成功", GroupName = Result.SuccessCode)]
        OrderSuccess = 100 + 10000,

        /// <summary>
        /// 订单票数据异常
        /// </summary>
        [Display(Name = "订单票数据异常")]
        TicketDataError = 101 + 10000,

        /// <summary>
        /// 订单日期有误
        /// </summary>
        [Display(Name = "订单日期有误")]
        OrderPlanDataError = 102 + 10000,

        /// <summary>
        /// 公园不存在
        /// </summary>
        [Display(Name = "公园不存在")]
        ParkNoExist = 103 + 10000,

        /// <summary>
        /// 订单已经存在
        /// </summary>
        [Display(Name = "订单已经存在")]
        OrderAlreadyExist = 104 + 10000,

        /// <summary>
        /// 取票码已经被使用
        /// </summary>
        [Display(Name = "取票码已经被使用")]
        PrepaymentLimit = 105 + 10000,

        /// <summary>
        /// 订单中身份证订票数量超出限制
        /// </summary>
        [Display(Name = "订单中身份证订票数量超出限制")]
        OrderIdnumUseLimit = 106 + 10000,

        /// <summary>
        /// 订单中票类无效
        /// </summary>
        [Display(Name = "订单中票类无效")]
        OrderTicketTypeIdError = 107 + 10000,

        /// <summary>
        /// 订单中身份证已订过票
        /// </summary>
        [Display(Name = "订单中身份证已订过票")]
        OrderIdnumUseAlready = 108 + 10000,

        /// <summary>
        /// 身份证格式错误
        /// </summary>
        [Display(Name = "身份证格式错误")]
        IdnumError = 109 + 10000,

        /// <summary>
        /// 订单票价错误
        /// </summary>
        [Display(Name = "订单票价错误")]
        OrderTicketPriceError = 110 + 10000,

        /// <summary>
        /// 订单总价错误
        /// </summary>
        [Display(Name = "订单价格错误")]
        OrderTotalPriceError = 111 + 10000,

        /// <summary>
        /// 订单中只能有一张身份证
        /// </summary>
        [Display(Name = "订单中只能有一张身份证")]
        OrderIdnumLimit = 112 + 10000,

        /// <summary>
        /// 订单数据不符合要求
        /// </summary>
        [Display(Name = "订单数据不符合要求")]
        OrderTicketDataError = 113 + 10000,

        /// <summary>
        /// 订单已支付
        /// </summary>
        [Display(Name = "订单已支付")]
        OrderTicketPayed = 114 + 10000,

        /// <summary>
        /// 订单已取消
        /// </summary>
        [Display(Name = "订单已取消")]
        OrderAlreadyCancel = 201 + 10000,

        /// <summary>
        /// 订单取消失败
        /// </summary>
        [Display(Name = "订单取消失败")]
        OrderCancelFail = 202 + 10000,

        /// <summary>
        /// 未支付订单不允许退票
        /// </summary>
        [Display(Name = "未支付订单不允许退票")]
        OrderCancelFailForNoPay = 203 + 10000,

        /// <summary>
        /// 订单取消成功
        /// </summary>
        [Display(Name = "订单取消成功")]
        OrderCancelSuccess = 204 + 10000,

        /// <summary>
        /// 订单取消失败，订单号不存在
        /// </summary>
        [Display(Name = "订单取消失败，订单号不存在")]
        OrderCancelFailOrderidNoExist = 205 + 10000,

        /// <summary>
        /// 订单取消失败，订单号不存在
        /// </summary>
        [Display(Name = "订单取消失败，订单号不存在")]
        OrderCancelFailParkOrderidNoExist = 206 + 10000,

        /// <summary>
        /// 订单取消失败，订单已使用
        /// </summary>
        [Display(Name = "订单取消失败，订单已使用")]
        OrderCancelFailAlreadyUsed = 207 + 10000,

        /// <summary>
        /// 订单取消失败，订单已使用
        /// </summary>
        [Display(Name = "订单取消失败，订单已冻结")]
        OrderCancelFailFrozen = 209 + 10000,

        /// <summary>
        /// 订单已经取消，不允许修改
        /// </summary>
        [Display(Name = "订单已经取消，不允许修改")]
        OrderUpdateFailAlreadyCancel = 301 + 10000,

        /// <summary>
        /// 订单已经使用，不允许修改
        /// </summary>
        [Display(Name = "订单已经使用，不允许修改")]
        OrderUpdateFailAlreadyUsed = 302 + 10000,

        /// <summary>
        /// 库存已售馨
        /// </summary>
        [Display(Name = "库存已售馨")]
        TicketSaleOut = 303 + 10000,

        #endregion



    }
}