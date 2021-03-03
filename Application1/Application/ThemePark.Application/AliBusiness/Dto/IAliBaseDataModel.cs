
namespace ThemePark.Application.AliBusiness.Dto
{
    /// <summary>
    /// 阿里推送过来的必选参数
    /// </summary>
    public interface AliBaseDataModel
    {
        #region Require Properties

        /// <summary>
        /// 请求类型，通过此参数区分不同的通知请求
        /// </summary>
        string Method { get; set; }

        /// <summary>
        /// 电子凭证与码商交互协议的版本号
        /// </summary>
        string Version { get; set; }

        /// <summary>
        /// 请求id，唯一标识一次请求
        /// </summary>
        string RequestId { get; set; }

        /// <summary>
        /// 接口调用时间
        /// </summary>
        string Timestamp { get; set; }

        /// <summary>
        /// 参数编码的字符集，默认为GBK
        /// </summary>
        string Charset { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        string Sign { get; set; }

        /// <summary>
        /// 是否为全链路压测流量
        /// </summary>
        string IsPerfReq { get; set; }

        /// <summary>
        /// 外部业务ID，交易域就是订单ID，子订单ID
        /// </summary>
        string OuterId { get; set; }

        /// <summary>
        /// token验证串，商家回调时须回传，否则将验证不通过，同一个订单的token是唯一的，不会变
        /// </summary>
        string Token { get; set; }

        /// <summary>
        /// 标的物提供者的编号，交易域为卖家id
        /// </summary>
        string OrganizerId { get; set; }

        /// <summary>
        /// 标的物提供者的名称，交易域为卖家nick
        /// </summary>
        string OrganizerNick { get; set; }

        /// <summary>
        /// 码的有效期开始时间:yyyy-MM-dd HH:mm:ss
        /// </summary>
        string ValidStart { get; set; }

        /// <summary>
        /// 码的有效期结束时间：yyyy-MM-dd HH:mm:ss
        /// </summary>
        string ValidEnd { get; set; }

        #endregion

        #region Optional

        /// <summary>
        /// 电子凭证持有者手机号，可以理解为买家手机号码
        /// </summary>
        string Mobile { get; set; }

        #endregion
    }
}
