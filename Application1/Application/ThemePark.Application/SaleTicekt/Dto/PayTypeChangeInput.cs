namespace ThemePark.Application.SaleTicekt.Dto
{
    public class PayTypeChangeInput
    {
        /// <summary>
        /// 转换类型
        /// </summary>
        public ChangeType ChangeType { get; set; }

        /// <summary>
        /// 交易号
        /// </summary>
        public string TradeInfoId { get; set; }
    }



    /// <summary>
    /// 支付方式转换
    /// </summary>
    public enum ChangeType
    {
        /// <summary>
        /// 现金转银行卡
        /// </summary>
        CashToBankCard = 0,

        /// <summary>
        /// 银行卡转现金
        /// </summary>
        BankCardToCash = 1,

        /// <summary>
        /// 现金转预存款
        /// </summary>
        CashToPrePay = 2,

        /// <summary>
        /// 预存款转现金
        /// </summary>
        PrePayToCash = 3,
        /// <summary>
        /// 现金转挂账
        /// </summary>
        CashToAccound = 4,

        /// <summary>
        /// 挂账转现金
        /// </summary>
        AccoundToCash = 5
    }
}
