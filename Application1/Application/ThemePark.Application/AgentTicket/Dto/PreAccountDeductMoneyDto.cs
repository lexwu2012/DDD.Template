namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 中心预付款扣除dto
    /// </summary>
    public class PreAccountDeductMoneyDto
    {
        /// <summary>
        /// 代理商Id
        /// </summary>
        public int AgencyId { get; set; }

        /// <summary>
        /// 订单Id
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// 扣减金额
        /// </summary>
        public decimal TotalMoney { get; set; }
    }
}
