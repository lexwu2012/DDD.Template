namespace ThemePark.Application.OTA.DTO
{
    public class OTAPayOrderDto
    {
        /// <summary>
        /// 主订单号
        /// </summary>
        public string TOHeaderId { get; set; }

        /// <summary>
        /// 取票二维码图片
        /// </summary>
        public string QRCode { get; set; }
    }
}
