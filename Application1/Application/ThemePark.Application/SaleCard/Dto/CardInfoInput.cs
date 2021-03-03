namespace ThemePark.Application.SaleCard.Dto
{
    /// <summary>
    /// 年卡激活卡信息
    /// </summary>
    public class CardInfoInput
    {
        /// <summary>
        /// 激活类别 1 年卡 2凭证 3电子年卡
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 年卡卡内码
        /// </summary>
        public string Icno { get; set; }

        /// <summary>
        /// vipCardId
        /// </summary>
        public int VipCardId { get; set; }

        /// <summary>
        /// 凭证条码
        /// </summary>
        public string Barcode { get; set; }

    }
}
