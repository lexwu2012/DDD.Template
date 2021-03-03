
using System.Text;
using System.Web;

namespace ThemePark.Application.AliBusiness.Dto
{
    /// <summary>
    /// 
    /// </summary>
    public class AliParamsCollection : SendAndResendNotificationDto, SuitNotificationDto, RefundNotificationDto, ModifyIdCardNotificationDto, ModifyMobileNotificationDto
    {
        private string organiztionName;
        private string title;
        private string biz;

        /// <summary>
        /// 
        /// </summary>
        public string SendStyle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ItemTitle
        {
            get { return title; }
            set { title = HttpUtility.UrlDecode(value, Encoding.GetEncoding("GBK")); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string MainOrderId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string MobileType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string IdCard { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string IdCardType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TotalFee { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Weeks { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SkuProperties { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string BizExtend
        {
            get { return biz; }
            set { biz = HttpUtility.UrlDecode(value, Encoding.GetEncoding("GBK")); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string OuterIdSKU { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Timestamp { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Charset { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Sign { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string IsPerfReq { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OuterId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OrganizerId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OrganizerNick
        {
            get { return organiztionName; }
            set { organiztionName = HttpUtility.UrlDecode(value, Encoding.GetEncoding("GBK")); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ValidStart { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ValidEnd { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AvailableNum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SuitResult { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string RefundFee { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int CancelNum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OuterIdP { get; set; }

    }


    public interface SendAndResendNotificationDto : AliBaseDataModel
    {
        #region Required Fields

        /// <summary>
        /// 发码风格,0表示一码一刷，1表示一码多刷
        /// </summary>
        string SendStyle { get; set; }

        /// <summary>
        /// 发码份数,可以理解为商品购买数量
        /// </summary>
        int Amount { get; set; }

        /// <summary>
        /// 标的物编号，交易域为商品id
        /// </summary>
        string ItemId { get; set; }

        /// <summary>
        /// 标的物名称描述，交易域为商品title
        /// </summary>
        string ItemTitle { get; set; }

        #endregion


        #region Optional Fields

        /// <summary>
        /// 主订单id
        /// </summary>
        string MainOrderId { get; set; }

        /// <summary>
        /// 手机号码类型，为0时mobile为明文数据，码商系统需要发送验证码至消费者手机；
        /// 为1时mobile为md5加密后数据，则回调发码后验证码短信由淘宝平台负责发送；
        /// 为2时mobile为明文数据，短信也由淘宝平台发送
        /// </summary>
        string MobileType { get; set; }

        /// <summary>
        /// 电子凭证持有者身份证号，可以理解为买家身份证号码。
        /// </summary>
        string IdCard { get; set; }

        /// <summary>
        /// 身份证号码类型，为1时idCard为md5加密后的数据，为2时idCard为对称加密后的身份证
        /// </summary>
        string IdCardType { get; set; }

        /// <summary>
        /// 交易订单实付金额，需由卖家授权才能获取到此数据(在商家管理-> 商家工具-> 参数设置)
        /// </summary>
        string TotalFee { get; set; }

        /// <summary>
        /// 航旅门票有效期中的星期设置信息
        /// </summary>
        string Weeks { get; set; }

        //电子凭证祝福语
        //public string blessWords { get; set; }

        /// <summary>
        /// 淘宝的宝贝sku文本信息
        /// </summary>
        string SkuProperties { get; set; }

        /// <summary>
        /// bizExtend=[{"buyerNick":"ohhenry","idCard":"y5QeNWKoFWhe9HzgpQEsf57iax3q4RH7","mobileNo":"UE2RmeF9TITPNDhvlyo0+w==","name":"朱海宁"}]
        /// </summary>
        string BizExtend { get; set; }

        /// <summary>
        /// 发布商品时，sku处填写的商家编码
        ///  !!! 通过这个字段取得商品ID, 这个字段格式为productId_T0
        /// </summary>
        string OuterIdSKU { get; set; }

        #endregion

        #region Resend Require Fields

        //码剩余可用数
        string AvailableNum { get; set; }

        #endregion

    }

    /// <summary>
    /// 维权通知参数
    /// </summary>
    public interface SuitNotificationDto : AliBaseDataModel
    {
        #region Require Properties

        /// <summary>
        /// 维权结果(1表示维权成功，后续会增加新的类型)
        /// </summary>
        string SuitResult { get; set; }

        /// <summary>
        /// 商品title
        /// </summary>
        string ItemTitle { get; set; }

        #endregion

        #region Optional Properties

        /// <summary>
        /// 退款金额，单位是元
        /// </summary>
        string RefundFee { get; set; }

        #endregion
    }

    public interface RefundNotificationDto : AliBaseDataModel
    {
        #region Require properties

        //发码风格,0表示一码一刷，1表示一码多刷
        /// <summary>
        /// 
        /// </summary>
        string SendStyle { get; set; }

        //发码份数,可以理解为商品购买数量
        int Amount { get; set; }

        //标的物编号，交易域为商品id
        string ItemId { get; set; }

        //标的物名称描述，交易域为商品title
        string ItemTitle { get; set; }

        #region Self Property
        //取消的码的个数
        int CancelNum { get; set; }

        #endregion

        #endregion

        #region Optional

        //退款金额，单位是元
        string RefundFee { get; set; }

        //发布商品时填写的商家编码
        string OuterIdP { get; set; }

        /*
         *   发布商品时，sku处填写的商家编码
         *  !!! 通过这个字段取得商品ID, 这个字段格式为productId_T0
        */
        string OuterIdSKU { get; set; }

        //手机号码类型，为0时mobile为明文数据，码商系统需要发送验证码至消费者手机；为1时mobile为md5加密后数据，则回调发码后验证码短信由淘宝平台负责发送；为2时mobile为明文数据，短信也由淘宝平台发送
        string MobileType { get; set; }

        //电子凭证持有者身份证号，可以理解为买家身份证号码。
        string IdCard { get; set; }

        //身份证号码类型，为1时idCard为md5加密后的数据，为2时idCard为对称加密后的身份证
        string IdCardType { get; set; }


        #endregion
    }

    public interface ModifyIdCardNotificationDto : AliBaseDataModel
    {
        #region Require Properties

        /// <summary>
        /// 标的物编号，交易域为商品id
        /// </summary>
        string ItemId { get; set; }

        /// <summary>
        /// 标的物名称描述，交易域为商品title
        /// </summary>
        string ItemTitle { get; set; }

        #endregion

        #region Optional

        /// <summary>
        /// 电子凭证持有者身份证号，可以理解为买家身份证号码。
        /// </summary>
        string IdCard { get; set; }

        /// <summary>
        /// 身份证号码类型，为1时idCard为md5加密后的数据，为2时idCard为对称加密后的身份证
        /// </summary>
        string IdCardType { get; set; }

        #endregion
    }

    /// <summary>
    /// 修改手机号通知dto
    /// </summary>
    public interface ModifyMobileNotificationDto : AliBaseDataModel
    {
        /// <summary>
        /// 手机号码类型，为0时mobile为明文数据，码商系统需要发送验证码至消费者手机；
        /// 为1时mobile为md5加密后数据，则回调发码后验证码短信由淘宝平台负责发送；为2时mobile为明文数据，短信也由淘宝平台发送
        /// </summary>
        string MobileType { get; set; }

        /// <summary>
        /// 发码份数,可以理解为商品购买数量
        /// </summary>
        int Amount { get; set; }

        /// <summary>
        /// 主订单号
        /// </summary>
        string MainOrderId { get; set; }
    }

    /// <summary>
    /// 通用扩展属性，包含买家昵称、手机号码等数据
    /// </summary>
    public class BizExtend
    {
        /// <summary>
        /// 买家昵称
        /// </summary>
        public string BuyerNick { get; set; }

        /// <summary>
        /// 买家身份证
        /// </summary>
        public string IdCard { get; set; }

        /// <summary>
        /// 买家手机号码
        /// </summary>
        public string MobileNo { get; set; }

        /// <summary>
        /// 买家真实名字
        /// </summary>
        public string Name { get; set; }
    }
}
