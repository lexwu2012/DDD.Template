namespace ThemePark.Application.AliBusiness.Dto
{
    /// <summary>
    /// 订票返回的数据
    /// </summary>
    public class OrderTicketReturnDto
    {
        /// <summary>
        /// 票务系统返回的订单号
        /// </summary>
        public string Orderid { get; set; }

        /// <summary>
        /// 返回的取票码
        /// </summary>
        public string Ticketcode { get; set; }

        /// <summary>
        /// 身份证
        /// </summary>
        public string Idnum { get; set; }

        /// <summary>
        /// 票类
        /// </summary>
        public string Tickettypeid { get; set; }       
    }   
}
