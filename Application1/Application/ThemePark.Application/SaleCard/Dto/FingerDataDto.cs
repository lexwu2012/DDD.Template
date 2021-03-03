namespace ThemePark.Application.SaleCard.Dto
{

    /// <summary>
    /// 指纹验证数据
    /// </summary>
    public class FingerDataDto
    {
        /// <summary>
        /// VipCardId
        /// </summary>
        public int VipCardId { get; set; }

        /// <summary>
        /// 指纹编号
        /// </summary>
        public int EnrollId { get; set; }
        /// <summary>
        /// TXW指纹
        /// </summary>
        public object Finger { get; set; }

        /// <summary>
        /// ZK指纹
        /// </summary>
        public object ZkFinger { get; set; }

        /// <summary>
        /// IC卡内码/条码
        /// </summary>
        public string IcBarcode { get; set; }

        /// <summary>
        /// 身份证
        /// </summary>
        public string Idnum { get; set; }

        /// <summary>
        /// 年卡/票类名称
        /// </summary>
        public string TicketClassName { get; set; }

    }
}
