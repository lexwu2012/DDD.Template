using System.ComponentModel.DataAnnotations;

namespace ThemePark.Application.Payment.Dto
{
    /// <summary>
    /// 支付平台状态码
    /// </summary>
    public enum PaymentResultCode
    {
        /// <summary>
        /// 操作成功
        ///</summary>
        [Display(Name = "操作成功")]
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
        /// 签名验证失败
        /// </summary>
        [Display(Name = "签名验证失败")]
        InvalidSign = 23,

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
        MissEssentialData = 406
    }
}
