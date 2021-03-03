

namespace ThemePark.VerifyTicketDto.Model
{
    public class CheckFingerModel
    {
        public string VerifyCode { get; set; }
        public string Id { get; set; }
        public byte[] FingerData { get; set; }
        public ZWJType FingerType { get; set; }
        public int Terminal { get; set; }
    }

    /// <summary>
    /// 指纹机型号
    /// </summary>
    public enum ZWJType
    {
        /// <summary>
        /// 中控指纹机
        /// </summary>
        ZK = 0,  // 中控指纹机

        /// <summary>
        /// 
        /// </summary>
        LUM = 1,//

        /// <summary>
        /// 通讯威指纹机
        /// </summary>
        TXW = 2  // 通讯威指纹机
    }
}