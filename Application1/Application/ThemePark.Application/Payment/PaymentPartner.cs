namespace ThemePark.Application.Payment
{
    /// <summary>
    /// 支付合作平台
    /// </summary>
    public class PaymentPartner
    {
        /// <summary>
        /// 支付合作平台
        /// </summary>
        public PaymentPartner()
        {
            
        }

        /// <summary>
        /// 支付合作平台
        /// </summary>
        public PaymentPartner(string partnerCode, string innerRsaPrivateKey)
        {
            PartnerCode = partnerCode;
            InnerRsaPrivateKey = innerRsaPrivateKey;
        }

        /// <summary>
        /// 合作平台身份标识
        /// </summary>
        public string PartnerCode { get; set; }

        /// <summary>
        /// 内部Rsa签名私钥
        /// </summary>
        public string InnerRsaPrivateKey { get; set; }
    }
}