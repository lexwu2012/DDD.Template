namespace ThemePark.VerifyTicketDto.Dto
{
    /// <summary>
    /// 验订单返回结果
    /// </summary>
    public class VerifyOrderDto
    {
        /// <summary>
        /// 订单来源电商
        /// </summary>
        public string OTA { get; set; }

        /// <summary>
        /// 游客姓名
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 条码/IC卡号等唯一码	
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 闸机上显示的票类名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 可过闸人数
        /// </summary>
        public int Persons { get; set; }

        /// <summary>
        /// 备注
        /// </summary>        
        public string Remark { get; set; }
    }
}
