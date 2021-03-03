
namespace ThemePark.Application.AliBusiness.Dto
{
    /// <summary>
    /// 方特系统订单详情dto
    /// </summary>
    public class FetchFtOrderDetailDto : OrderTicketReturnDto
    {
        /// <summary>
        /// 入园日期
        /// </summary>
        public string Plandate { get; set; }

        /// <summary>
        /// 订单状态（0:未出票 2:已出票  3：已取消）
        /// </summary>
        public string Planstate { get; set; }

        /// <summary>
        /// 核销份数
        /// </summary>
        public int Number { get; set; }
    }

    /// <summary>
    /// 获取方特系统订单详情
    /// </summary>
    public class OrderTicketDetailEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public FetchFtOrderDetailDto[] Data { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ResultState ResultStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Time { get; set; }
    }
}
