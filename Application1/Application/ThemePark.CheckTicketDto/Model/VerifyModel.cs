using ThemePark.VerifyTicketDto.Dto;

namespace ThemePark.VerifyTicketDto.Model
{
    /// <summary>
    /// 验证实体
    /// </summary>
    public class VerifyModel
    {
        /// <summary>
        /// 验证设备类型
        /// </summary>
        public VerifyType VerifyType { get; set; }

        /// <summary>
        /// 读码
        /// </summary>
        public string VerifyCode { get; set; }

        /// <summary>
        /// 闸机号
        /// </summary>
        public int Terminal { get; set; }
    }

}