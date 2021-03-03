using System.ComponentModel.DataAnnotations;

namespace ThemePark.Infrastructure.Enumeration
{
    /// <summary>
    /// 报表url枚举
    /// </summary>
    public enum ReportUrlEnum
    {
        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "客户端报表")]
        Report = 1,

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "中心报表")]
        CenterReport = 2,

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "旅行社报表")]
        TravelReport = 3,

        #region 客户端报表

        /// <summary>
        /// 销售--销售汇总
        /// </summary>
        [Display(Name = "销售--销售汇总", Description = "销售汇总")]
        SaleSummary = 10,

        /// <summary>
        /// 销售--散客销售明细
        /// </summary>
        [Display(Name = "销售--散客销售明细", Description = "散客销售明细")]
        SaleNonGroupDetail = 15,

        /// <summary>
        /// 销售--团体销售明细
        /// </summary>
        [Display(Name = "销售--团体销售明细", Description = "团体销售明细")]
        SaleGroupDetail = 20,

        /// <summary>
        /// 销售--团体月报
        /// </summary>
        [Display(Name = "销售--团体月报", Description = "团体月报")]
        SaleGroupMonth = 25,

        /// <summary>
        /// 销售--年卡综合报表
        /// </summary>
        [Display(Name = "销售--年卡综合报表", Description = "年卡综合报表")]
        SaleYearCardSale = 30,

        /// <summary>
        /// 销售--分成销售明细
        /// </summary>
        [Display(Name = "销售--分成销售明细", Description = "分成销售明细")]
        SaleByOtherPark = 31,

        /// <summary>
        /// 收银员--收银员交易明细
        /// </summary>
        [Display(Name = "收银员--收银员交易明细", Description = "收银员交易明细")]
        CashierTradeDetail = 40,

        /// <summary>
        /// 收银员--收银员交易汇总
        /// </summary>
        [Display(Name = "收银员--收银员交易汇总", Description = "收银员交易汇总")]
        CashierTradeSummary = 50,

        /// <summary>
        /// 入园--入园报告
        /// </summary>
        [Display(Name = "入园--入园报告", Description = "入园报告")]
        InParkReport = 60,

        /// <summary>
        /// 入园--入园明细
        /// </summary>
        [Display(Name = "入园--入园明细", Description = "入园明细")]
        InParkDetail = 61,

        /// <summary>
        /// 入园--入园时段统计
        /// </summary>
        [Display(Name = "入园--入园时段统计", Description = "入园时段统计")]
        InParkHours = 62,


        /// <summary>
        /// 入园--年卡入园明细
        /// </summary>
        [Display(Name = "入园--年卡入园明细", Description = "年卡入园明细")]
        InParkYearCard = 65,

        /// <summary>
        /// 入园--管理卡入园明细
        /// </summary>
        [Display(Name = "入园--管理卡入园明细", Description = "管理卡入园明细")]
        InParkManagerCard = 70,

        /// <summary>
        /// 入园--入园单明细查询
        /// </summary>
        [Display(Name = "入园--入园单明细查询", Description = "入园单明细查询")]
        InParkBill = 75,

        ///// <summary>
        ///// 团体销售明细 (财务)
        ///// </summary>
        //[Display(Name = "团体销售明细 (财务)", Description = "团体销售明细 (财务)")]
        //FinanceGroupSaleDetail = 80,

        /// <summary>
        /// ota--网络预订明细
        /// </summary>
        [Display(Name = "ota--网络预订明细", Description = "网络预订明细")]
        OTAReserveOrderDetail = 85,

        /// <summary>
        /// ota--网络销售明细
        /// </summary>
        [Display(Name = "ota--网络销售明细", Description = "网络销售明细")]
        OTAOutTicketDetail = 90,

        /// <summary>
        /// 查询--条码重打印查询
        /// </summary>
        [Display(Name = "查询--条码重打印查询", Description = "条码重打印查询")]
        QueryReprint = 95,      

        /// <summary>
        /// 查询--退票汇总查询报表
        /// </summary>
        [Display(Name = "查询--退票汇总查询", Description = "退票汇总查询")]
        QueryRefundSummary = 100,     

        /// <summary>
        /// 查询--补票查询
        /// </summary>
        [Display(Name = "查询--补票查询", Description = "补票查询")]
        QueryExcessFare = 105,

        /// <summary>
        /// 查询--网络订单查询（去掉了这个报表）
        /// </summary>
        //[Display(Name = "查询--网络订单查询", Description = "网络订单查询")]
        //QueryOnlineOrder = 110,

        /// <summary>
        /// 查询--预订查询
        /// </summary>
        [Display(Name = "查询--预订查询", Description = "预订查询")]
        QueryGroupOrder = 115,

        /// <summary>
        /// 查询--发票查询
        /// </summary>
        [Display(Name = "查询--发票查询", Description = "发票查询")]
        QueryInvoiceReport = 120,

        /// <summary>
        /// 查询--年卡挂失解挂明细
        /// </summary>
        [Display(Name = "查询--年卡挂失解挂明细", Description = "年卡挂失解挂明细")]
        QueryICLossOp = 125,

        /// <summary>
        /// 查询--支付交易明细查询
        /// </summary>
        [Display(Name = "查询--支付交易明细查询", Description = "支付交易明细查询")]
        QueryTradeInfoDetail = 130,

        /// <summary>
        /// 查询--已售未入园查询
        /// </summary>
        [Display(Name = "查询--已售未入园查询", Description = "已售未入园查询")]
        QuerySoldNotInPark = 135,

        /// <summary>
        /// 团体挂账明细
        /// </summary>
        [Display(Name = "团体挂账明细", Description = "团体挂账明细")]
        GroupAccountOnDetail = 136,

        /// <summary>
        /// 审计--入园人数明细表 140
        /// </summary>
        [Display(Name = "审计--入园人数明细表", Description = "入园人数明细表")]
        AuditInParkPersonsCount = 140,

        ///// <summary>
        ///// 审计--售票明细表 145
        ///// </summary>
        //[Display(Name = "审计--售票明细表", Description = "售票明细表")]
        //AuditSaleTicketDetail = 145,

        /// <summary>
        /// 审计--售票汇总表 150
        /// </summary>
        [Display(Name = "审计--售票汇总表", Description = "售票汇总表")]
        AuditSaleTicketSummary = 150,





        #endregion

        #region 中心报表
        

        /// <summary>
        /// OTA--官方总体查询报表 200
        /// </summary>
        [Display(Name = "OTA--官方总体查询报表")]
        OTAOfficialSummary = 200,


        /// <summary>
        /// 一、OTA--官方订单明细 205
        /// </summary>
        [Display(Name = "OTA--官方订单明细")]
        OTAOfficialDetail = 205,

        /// <summary>
        /// 二、OTA--官方和旅游网订单明细 210
        /// </summary>
        [Display(Name = "OTA--官方和旅游网订单明细")]
        OTAOfficialAndOnlineDetail = 210,

        /// <summary>
        /// 三、OTA--旅游网总体查询 220
        /// </summary>
        [Display(Name = "OTA--旅游网总体查询")]
        OTAOnlineSummary = 220,

        /// <summary>
        /// 四、OTA--旅游网订单明细 225
        /// </summary>
        [Display(Name = "OTA--旅游网订单明细")]
        OTAOnlineDetail = 225,

        /// <summary>
        /// 五、OTA--各渠道网络销售汇总 230
        /// </summary>
        [Display(Name = "OTA--各渠道销售汇总")]
        OTASingleSource = 230,

        /// <summary>
        /// 六、OTA--各渠道网络销售汇总(按日期) 240
        /// </summary>
        [Display(Name = "OTA--各网络渠道销售汇总(按日期)")]
        OTASingleSourceByDate = 240,
     
        /// <summary>
        /// 七、OTA--各公园网络销售汇总 250
        /// </summary>
        [Display(Name = "OTA--各公园网络销售汇总(按渠道)")]
        OTAAllParkSummary = 250,

        /// <summary>
        /// 八、OTA--单公园网络销售汇总 260
        /// </summary>
        [Display(Name = "OTA--单公园网络销售汇总")]
        OTASingleParkSummary = 260,

        /// <summary>
        /// 九、OTA--单个产品网络销售统计 270
        /// </summary>
        [Display(Name = "OTA--单个产品网络销售统计")]
        OTASingleProduct = 270,

        /// <summary>
        ///  旅行社综合报表
        /// </summary>
        [Display(Name = "旅行社综合报表")]
        TravelAgencyIntegrateReport = 280,

        /// <summary>
        /// 旅行社日报表汇总
        /// </summary>
        [Display(Name = "旅行社日报表汇总")]
        TravelAgencyMonthReport = 290,

        /// <summary>
        /// 旅行社订单明细
        /// </summary>
        [Display(Name = "旅行社订单明细")]
        TravelAgencyOrderDetail = 300,

        /// <summary>
        /// OTA--OTA促销票类信息 310
        /// </summary>
        [Display(Name = "OTA促销票类信息")]
        OTASaleTicketClassInfo = 310,


        /// <summary>
        /// OTA--OTA订单手机号管理 320
        /// </summary>
        [Display(Name = "OTA订单手机号管理")]
        OTAOrderPhoneNumber = 320,

        /// <summary>
        /// Summary--总体销售分析 330
        /// </summary>
        [Display(Name = "Summary--总体销售分析")]
        SummarySalesAnalysis = 330,


        /// <summary>
        /// Summary--公园销售分析 340
        /// </summary>
        [Display(Name = "Summary--公园销售分析")]
        SummarySalesAnalysisOnePark = 340,

        #endregion  中心报表



        #region 旅行社报表
        /// <summary>
        /// 测试报表
        /// </summary>
        [Display(Name = "测试报表")]
        TestReport = 199, 

        /// <summary>
        ///  旅行社报表
        /// </summary>
        [Display(Name = "TravelSystem--旅行社综合报表")]
        TravelAgencyReport = 500,

        #endregion
    }
}
