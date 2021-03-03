namespace ThemePark.VerifyTicketDto.Dto
{
    /// <summary>
    /// 验票类型
    /// </summary>
    public enum VerifyType
    {
        #region 客户端请求的类型

        /// <summary>
        /// 条码
        /// </summary>
        Barcode = 1,    

        /// <summary>
        /// 身份证号
        /// </summary>
        PID = 2,        

        /// <summary>
        /// IC卡号
        /// </summary>
        ICNo = 3,       

        /// <summary>
        /// 指纹
        /// </summary>
        Finger = 4 ,    

        /// <summary>
        /// 二维码
        /// </summary>
        QRCode = 5,

        /// <summary>
        /// 刷脸
        /// </summary>
        Face,

        #endregion

        #region 服务端返回的类型

        /// <summary>
        /// 普通条码票
        /// </summary>
        CommonTicket = 100,

        /// <summary>
        /// 套票
        /// </summary>
        MultiTicket = 110,

        /// <summary>
        /// 入园单
        /// </summary>
        InparkBill = 120,

        /// <summary>
        /// 在线订单
        /// </summary>
        Order = 130,

        /// <summary>
        /// 年卡
        /// </summary>                 
        VIPCard = 140,

        /// <summary>
        /// 多园年卡
        /// </summary>
        MultiYearCard = 145,

        /// <summary>
        /// 管理卡
        /// </summary>                  
        ManageCard = 150,

        /// <summary>
        /// 二次入园管理卡
        /// </summary>
        SecondCard = 160,
        
        /// <summary>
        /// 二次入园票
        /// </summary>
        SecondTicket = 170,

        /// <summary>
        /// 条码票列表
        /// </summary>
        Barcodes = 180,

        /// <summary>
        /// 无效指纹
        /// </summary>
        InvalidFinger=190,

        /// <summary>
        /// 无效票
        /// </summary>
        InvalidTicket = 400
        #endregion
    }

    /// <summary>
    /// 验票返回的数据对象
    /// </summary>
    public class VerifyDto
    {
        /// <summary>
        /// 验票的编码
        /// </summary>
        public string VerifyCode { get; set; }

        /// <summary>
        /// 验票类型
        /// </summary>
        public VerifyType VerifyType { get; set; }

        /// <summary>
        /// 验票结果
        /// </summary>
        public string VerifyData { get; set; } // json
    }

}
