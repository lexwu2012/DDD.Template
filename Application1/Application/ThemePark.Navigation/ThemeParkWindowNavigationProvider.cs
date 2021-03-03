using Abp.Application.Navigation;
using Abp.Localization;
using ThemePark.Common;
using ThemePark.Core;
using ThemePark.Core.Authorization;
using ThemePark.Infrastructure.Enumeration;

namespace ThemePark.Navigation
{
    public class ThemeParkWindowNavigationProvider : NavigationProvider
    {
        public const string MenuName = "WindowMenue";

        public override void SetNavigation(INavigationProviderContext context)
        {
            context.Manager.Menus.Add(MenuName, new MenuDefinition(MenuName, new FixedLocalizableString("Ticket menu")));

            context.Manager.Menus[MenuName]
                .AddItem(
                    new MenuItemDefinition(
                        PermissionNames.Window_SaleTicket,
                        L(PermissionNames.Window_SaleTicket),
                        icon: "fa fa-users",
                        url: "sale",
                        requiresAuthentication: true,
                        requiredPermissionName: PermissionNames.Window_SaleTicket
                        )
                        .AddItem(
                            new MenuItemDefinition(
                                PermissionNames.Window_SaleTicket_NonGroup,
                                L(PermissionNames.Window_SaleTicket_NonGroup),
                                icon: null,
                                url: "ticket.module.nonGroup",
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Window_SaleTicket_NonGroup
                                ))
                        .AddItem(
                            new MenuItemDefinition(
                                PermissionNames.Window_SaleTicket_Group,
                                L(PermissionNames.Window_SaleTicket_Group),
                                icon: null,
                                url: "ticket.module.group",
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Window_SaleTicket_Group
                                ))
                        .AddItem(
                            new MenuItemDefinition(
                                PermissionNames.Window_SaleTicket_Web,
                                L(PermissionNames.Window_SaleTicket_Web),
                                icon: null,
                                url: "ticket.module.webTicket",
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Window_SaleTicket_Web
                                ))
                        .AddItem(
                            new MenuItemDefinition(
                                PermissionNames.Window_SaleTicket_Reprint,
                                L(PermissionNames.Window_SaleTicket_Reprint),
                                icon: null,
                                url: "ticket.module.reprint",
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Window_SaleTicket_Reprint
                                ))
                       .AddItem(
                            new MenuItemDefinition(
                                PermissionNames.Window_SaleTicket_FareAdjustment,
                                L(PermissionNames.Window_SaleTicket_FareAdjustment),
                                icon: null,
                                url: "ticket.module.fareAdjustment",
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Window_SaleTicket_FareAdjustment
                                ))
                        .AddItem(
                            new MenuItemDefinition(
                                PermissionNames.Window_SaleTicket_FareAdjustmentLeader,
                                L(PermissionNames.Window_SaleTicket_FareAdjustmentLeader),
                                icon: null,
                                url: "ticket.module.fareAdjustmentLeader",
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Window_SaleTicket_FareAdjustmentLeader
                                ))
                       .AddItem(
                            new MenuItemDefinition(
                                PermissionNames.Window_SaleTicket_Refund,
                                L(PermissionNames.Window_SaleTicket_Refund),
                                icon: null,
                                url: "ticket.module.refund",
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Window_SaleTicket_Refund
                                ))
                       .AddItem(
                            new MenuItemDefinition(
                                PermissionNames.Window_SaleTicket_PreDeposit,
                                L(PermissionNames.Window_SaleTicket_PreDeposit),
                                icon: null,
                                url: "ticket.module.preDeposit",
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Window_SaleTicket_PreDeposit
                                ))
                        .AddItem(
                            new MenuItemDefinition(
                                PermissionNames.Window_SaleTicket_CheckCoupon,
                                L(PermissionNames.Window_SaleTicket_CheckCoupon),
                                icon: null,
                                url: "ticket.module.checkCoupon",
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Window_SaleTicket_CheckCoupon
                                ))
                        .AddItem(
                            new MenuItemDefinition(
                                PermissionNames.Window_SaleTicket_PayTypeChange,
                                L(PermissionNames.Window_SaleTicket_PayTypeChange),
                                icon: null,
                                url: "ticket.module.payTypeChange",
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Window_SaleTicket_PayTypeChange
                                ))
                )
                .AddItem(
                    new MenuItemDefinition(
                        PermissionNames.Window_YearCard,
                        L(PermissionNames.Window_YearCard),
                        icon: "fa fa-credit-card",
                        url: "yearcard",
                        requiresAuthentication: true,
                        requiredPermissionName: PermissionNames.Window_YearCard
                        )
                        .AddItem(
                            new MenuItemDefinition(
                                PermissionNames.Window_YearCard_CardSale,
                                L(PermissionNames.Window_YearCard_CardSale),
                                icon: null,
                                url: "ticket.module.cardSale",
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Window_YearCard_CardSale
                                ))
                        .AddItem(
                            new MenuItemDefinition(
                                PermissionNames.Window_YearCard_VoucherSale,
                                L(PermissionNames.Window_YearCard_VoucherSale),
                                icon: null,
                                url: "ticket.module.voucherSale",
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Window_YearCard_VoucherSale
                                ))
                        .AddItem(
                            new MenuItemDefinition(
                                PermissionNames.Window_YearCard_ActivateInfo,
                                L(PermissionNames.Window_YearCard_ActivateInfo),
                                icon: null,
                                url: "ticket.module.cardActivateInfo",
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Window_YearCard_ActivateInfo
                                ))
                        .AddItem(
                            new MenuItemDefinition(
                                PermissionNames.Window_YearCard_UserInfo,
                                L(PermissionNames.Window_YearCard_UserInfo),
                                icon: null,
                                url: "ticket.module.cardUserInfo",
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Window_YearCard_UserInfo
                                ))
                         .AddItem(
                            new MenuItemDefinition(
                                PermissionNames.Window_YearCard_LossReport,
                                L(PermissionNames.Window_YearCard_LossReport),
                                icon: null,
                                url: "ticket.module.cardLossReport",
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Window_YearCard_LossReport
                                ))
                         .AddItem(
                            new MenuItemDefinition(
                                PermissionNames.Window_YearCard_Reissue,
                                L(PermissionNames.Window_YearCard_Reissue),
                                icon: null,
                                url: "ticket.module.cardReissue",
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Window_YearCard_Reissue
                                ))
                         .AddItem(
                            new MenuItemDefinition(
                                PermissionNames.Window_YearCard_Renewal,
                                L(PermissionNames.Window_YearCard_Renewal),
                                icon: null,
                                url: "ticket.module.cardRenewal",
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Window_YearCard_Renewal
                                ))
                          .AddItem(
                            new MenuItemDefinition(
                                PermissionNames.Window_YearCard_Return,
                                L(PermissionNames.Window_YearCard_Return),
                                icon: null,
                                url: "ticket.module.cardReturn",
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Window_YearCard_Return
                                ))
                          .AddItem(
                            new MenuItemDefinition(
                                PermissionNames.Window_YearCard_VoucherReturn,
                                L(PermissionNames.Window_YearCard_VoucherReturn),
                                icon: null,
                                url: "ticket.module.voucherReturn",
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Window_YearCard_VoucherReturn
                                ))
                        )
                .AddItem(
                    new MenuItemDefinition(
                        PermissionNames.Window_ICCard,
                        L(PermissionNames.Window_ICCard),
                        icon: "fa fa-newspaper-o",
                        url: "iccard",
                        requiresAuthentication: true,
                        requiredPermissionName: PermissionNames.Window_ICCard
                        )
                        .AddItem(
                            new MenuItemDefinition(
                                PermissionNames.Window_ICCard_CardInit,
                                L(PermissionNames.Window_ICCard_CardInit),
                                icon: null,
                                url: "ticket.module.cardInit",
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Window_ICCard_CardInit
                                ))
                        )
                        .AddItem(
                    new MenuItemDefinition(
                        PermissionNames.Window_EnterBill,
                        L(PermissionNames.Window_EnterBill),
                        icon: "fa fa-file-text",
                        url: "enterbill",
                        requiresAuthentication: true,
                        requiredPermissionName: PermissionNames.Window_EnterBill
                        )
                        .AddItem(
                            new MenuItemDefinition(
                                PermissionNames.Window_EnterBill_BillPrint,
                                L(PermissionNames.Window_EnterBill_BillPrint),
                                icon: null,
                                url: "ticket.module.billPrint",
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Window_EnterBill_BillPrint
                                ))
                        .AddItem(
                            new MenuItemDefinition(
                                PermissionNames.Window_EnterBill_BillManage,
                                L(PermissionNames.Window_EnterBill_BillManage),
                                icon: null,
                                url: "ticket.module.billManage",
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Window_EnterBill_BillManage
                                ))
                         .AddItem(
                            new MenuItemDefinition(
                                PermissionNames.Window_EnterBill_BillManageLeader,
                                L(PermissionNames.Window_EnterBill_BillManageLeader),
                                icon: null,
                                url: "ticket.module.billManageLeader",
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Window_EnterBill_BillManageLeader
                                ))
                        )
                     .AddItem(
                         new MenuItemDefinition(
                            PermissionNames.Window_Report,
                            L(PermissionNames.Window_Report),
                            icon: "fa fa-table",
                            url: "report",
                            requiresAuthentication: true,
                            requiredPermissionName: PermissionNames.Window_Report
                            )
                            //销售--销售汇总
                            .AddItem(
                                new MenuItemDefinition(
                                    PermissionNames.Window_Report_SaleSummary,
                                    L(PermissionNames.Window_Report_SaleSummary),
                                    icon: null,
                                    url: Url("ticket.module.saleSummary", (int)ReportUrlEnum.SaleSummary, ReportUrlEnum.SaleSummary.DisplayDescription()),
                                    //url: "ticket.module.saleSummary",
                                    requiresAuthentication: true,
                                    requiredPermissionName: PermissionNames.Window_Report_SaleSummary
                                    ))
                             //销售--散客销售明细
                             .AddItem(
                                new MenuItemDefinition(
                                    PermissionNames.Window_Report_SaleNonGroupDetail,
                                    L(PermissionNames.Window_Report_SaleNonGroupDetail),
                                    icon: null,
                                    url: Url("ticket.module.saleNonGroupDetail", (int)ReportUrlEnum.SaleNonGroupDetail, ReportUrlEnum.SaleNonGroupDetail.DisplayDescription()),
                                    //url: "ticket.module.saleNonGroupDetail",
                                    requiresAuthentication: true,
                                    requiredPermissionName: PermissionNames.Window_Report_SaleNonGroupDetail
                                    ))
                              //销售--团体销售明细
                              .AddItem(
                                new MenuItemDefinition(
                                    PermissionNames.Window_Report_SaleGroupDetail,
                                    L(PermissionNames.Window_Report_SaleGroupDetail),
                                    icon: null,
                                    url: Url("ticket.module.saleGroupDetail", (int)ReportUrlEnum.SaleGroupDetail, ReportUrlEnum.SaleGroupDetail.DisplayDescription()),
                                    //url: "ticket.module.saleGroupDetail",
                                    requiresAuthentication: true,
                                    requiredPermissionName: PermissionNames.Window_Report_SaleGroupDetail
                                    ))
                              //销售--团体月报
                              .AddItem(
                                new MenuItemDefinition(
                                    PermissionNames.Window_Report_SaleGroupMonth,
                                    L(PermissionNames.Window_Report_SaleGroupMonth),
                                    icon: null,
                                    url: Url("ticket.module.saleGroupMonth", (int)ReportUrlEnum.SaleGroupMonth, ReportUrlEnum.SaleGroupMonth.DisplayDescription()),
                                    //url: "ticket.module.saleGroupDetail",
                                    requiresAuthentication: true,
                                    requiredPermissionName: PermissionNames.Window_Report_SaleGroupMonth
                                    ))
                              //销售--年卡综合报表
                              .AddItem(
                                new MenuItemDefinition(
                                    PermissionNames.Window_Report_SaleYearCardSale,
                                    L(PermissionNames.Window_Report_SaleYearCardSale),
                                    icon: null,
                                    url: Url("ticket.module.saleYearCardSale", (int)ReportUrlEnum.SaleYearCardSale, ReportUrlEnum.SaleYearCardSale.DisplayDescription()),
                                    requiresAuthentication: true,
                                    requiredPermissionName: PermissionNames.Window_Report_SaleYearCardSale
                                    ))
                              //销售--分成销售明细
                              .AddItem(
                                new MenuItemDefinition(
                                    PermissionNames.Window_Report_SaleByOtherPark,
                                    L(PermissionNames.Window_Report_SaleByOtherPark),
                                    icon: null,
                                    url: Url("ticket.module.saleByOtherPark", (int)ReportUrlEnum.SaleByOtherPark, ReportUrlEnum.SaleByOtherPark.DisplayDescription()),
                                    requiresAuthentication: true,
                                    requiredPermissionName: PermissionNames.Window_Report_SaleByOtherPark
                                    ))
                              //团体销售明细 (财务)
                              //.AddItem(
                              //  new MenuItemDefinition(
                              //      PermissionNames.Window_Report_FinanceGroupSaleDetail,
                              //      L(PermissionNames.Window_Report_FinanceGroupSaleDetail),
                              //      icon: null,
                              //      url: Url("ticket.module.financeGroupSaleDetail", (int)ReportUrlEnum.FinanceGroupSaleDetail, ReportUrlEnum.FinanceGroupSaleDetail.DisplayDescription()),
                              //      requiresAuthentication: true,
                              //      requiredPermissionName: PermissionNames.Window_Report_FinanceGroupSaleDetail
                              //      ))
                             // 收银员--收银员交易明细
                             .AddItem(
                              new MenuItemDefinition(
                                  PermissionNames.Window_Report_CashierTradeDetail,
                                  L(PermissionNames.Window_Report_CashierTradeDetail),
                                  icon: null,
                                  url: Url("ticket.module.cashierTradeDetail", (int)ReportUrlEnum.CashierTradeDetail, ReportUrlEnum.CashierTradeDetail.DisplayDescription()),
                                  requiresAuthentication: true,
                                  requiredPermissionName: PermissionNames.Window_Report_CashierTradeDetail
                                  ))
                            // 收银员--收银员交易汇总
                            .AddItem(
                              new MenuItemDefinition(
                                  PermissionNames.Window_Report_CashierTradeSummary,
                                  L(PermissionNames.Window_Report_CashierTradeSummary),
                                  icon: null,
                                  url: Url("ticket.module.cashierTradeSummary", (int)ReportUrlEnum.CashierTradeSummary, ReportUrlEnum.CashierTradeSummary.DisplayDescription()),
                                  //url: "ticket.module.cashierTradeSummary",
                                  requiresAuthentication: true,
                                  requiredPermissionName: PermissionNames.Window_Report_CashierTradeSummary
                                  ))
                            // 入园--入园报告
                            .AddItem(
                              new MenuItemDefinition(
                                  PermissionNames.Window_Report_InParkReport,
                                  L(PermissionNames.Window_Report_InParkReport),
                                  icon: null,
                                  url: Url("ticket.module.inParkReport", (int)ReportUrlEnum.InParkReport, ReportUrlEnum.InParkReport.DisplayDescription()),
                                  //"ticket.report.InParkDailyReport({type: '130'})",
                                  //url: "ticket.module.inParkReport",
                                  requiresAuthentication: true,
                                  requiredPermissionName: PermissionNames.Window_Report_InParkReport
                                  ))
                            // 入园--入园明细
                            .AddItem(
                              new MenuItemDefinition(
                                  PermissionNames.Window_Report_InParkDetail,
                                  L(PermissionNames.Window_Report_InParkDetail),
                                  icon: null,
                                  //url: Url("ticket.module.Report", (int)ReportUrlEnum.InParkDetail, ReportUrlEnum.InParkDetail.DisplayDescription()),
                                  //"ticket.report.InParkDailyReport({type: '130'})",//Url(ReportUrlEnum.InParkDailyReport.DisplayName()),
                                  url: "ticket.module.inParkDetail",
                                  requiresAuthentication: true,
                                  requiredPermissionName: PermissionNames.Window_Report_InParkDetail
                                  ))
                            // 入园--入园时段统计
                            .AddItem(
                              new MenuItemDefinition(
                                  PermissionNames.Window_Report_InParkHours,
                                  L(PermissionNames.Window_Report_InParkHours),
                                  icon: null,
                                  url: Url("ticket.module.inParkHours", (int)ReportUrlEnum.InParkHours, ReportUrlEnum.InParkHours.DisplayDescription()),
                                  requiresAuthentication: true,
                                  requiredPermissionName: PermissionNames.Window_Report_InParkHours
                                  ))
                            // 入园--年卡入园明细
                            .AddItem(
                              new MenuItemDefinition(
                                  PermissionNames.Window_Report_InParkYearCard,
                                  L(PermissionNames.Window_Report_InParkYearCard),
                                  icon: null,
                                  url: Url("ticket.module.inParkYearCard", (int)ReportUrlEnum.InParkYearCard, ReportUrlEnum.InParkYearCard.DisplayDescription()),
                                  requiresAuthentication: true,
                                  requiredPermissionName: PermissionNames.Window_Report_InParkYearCard
                                  ))
                           // 入园--管理卡入园查询
                           .AddItem(
                              new MenuItemDefinition(
                                  PermissionNames.Window_Report_InParkManagerCard,
                                  L(PermissionNames.Window_Report_InParkManagerCard),
                                  icon: null,
                                  url: Url("ticket.module.inParkManagerCard", (int)ReportUrlEnum.InParkManagerCard, ReportUrlEnum.InParkManagerCard.DisplayDescription()),
                                  requiresAuthentication: true,
                                  requiredPermissionName: PermissionNames.Window_Report_InParkManagerCard
                                  ))
                           // 入园--入园单明细查询
                           .AddItem(
                              new MenuItemDefinition(
                                  PermissionNames.Window_Report_InParkBill,
                                  L(PermissionNames.Window_Report_InParkBill),
                                  icon: null,
                                  url: Url("ticket.module.inParkBill", (int)ReportUrlEnum.InParkBill, ReportUrlEnum.InParkBill.DisplayDescription()),
                                  requiresAuthentication: true,
                                  requiredPermissionName: PermissionNames.Window_Report_InParkBill
                                  ))

                          // ota--网络预订明细
                          .AddItem(
                              new MenuItemDefinition(
                                  PermissionNames.Window_Report_OTAReserveOrderDetail,
                                  L(PermissionNames.Window_Report_OTAReserveOrderDetail),
                                  icon: null,
                                  url: Url("ticket.module.oTAReserveOrderDetail", (int)ReportUrlEnum.OTAReserveOrderDetail, ReportUrlEnum.OTAReserveOrderDetail.DisplayDescription()),
                                  requiresAuthentication: true,
                                  requiredPermissionName: PermissionNames.Window_Report_OTAReserveOrderDetail
                                  ))
                          // ota--网络销售明细
                          .AddItem(
                              new MenuItemDefinition(
                                  PermissionNames.Window_Report_OTAOutTicketDetail,
                                  L(PermissionNames.Window_Report_OTAOutTicketDetail),
                                  icon: null,
                                  url: Url("ticket.module.oTAOutTicketDetail", (int)ReportUrlEnum.OTAOutTicketDetail, ReportUrlEnum.OTAOutTicketDetail.DisplayDescription()),//"ticket.report.NonGroupTicketSaleDetail",
                                  requiresAuthentication: true,
                                  requiredPermissionName: PermissionNames.Window_Report_OTAOutTicketDetail
                                  ))
                          // 查询--条码重打印查询
                          .AddItem(
                              new MenuItemDefinition(
                                  PermissionNames.Window_Report_QueryReprint,
                                  L(PermissionNames.Window_Report_QueryReprint),
                                  icon: null,
                                  url: Url("ticket.module.queryReprint", (int)ReportUrlEnum.QueryReprint, ReportUrlEnum.QueryReprint.DisplayDescription()),//"ticket.report.OrderDetail",
                                  requiresAuthentication: true,
                                  requiredPermissionName: PermissionNames.Window_Report_QueryReprint
                                  ))
                            // 查询--退票汇总查询报表
                            .AddItem(
                              new MenuItemDefinition(
                                  PermissionNames.Window_Report_QueryRefundSummary,
                                  L(PermissionNames.Window_Report_QueryRefundSummary),
                                  icon: null,
                                  url: Url("ticket.module.queryRefundSummary", (int)ReportUrlEnum.QueryRefundSummary, ReportUrlEnum.QueryRefundSummary.DisplayDescription()),//"ticket.report.OrderDetail",
                                  requiresAuthentication: true,
                                  requiredPermissionName: PermissionNames.Window_Report_QueryRefundSummary
                                  ))
                           // 查询--补票查询
                           .AddItem(
                              new MenuItemDefinition(
                                  PermissionNames.Window_Report_QueryExcessFare,
                                  L(PermissionNames.Window_Report_QueryExcessFare),
                                  icon: null,
                                  url: Url("ticket.module.queryExcessFare", (int)ReportUrlEnum.QueryExcessFare, ReportUrlEnum.QueryExcessFare.DisplayDescription()),//"ticket.report.OrderDetail",
                                  requiresAuthentication: true,
                                  requiredPermissionName: PermissionNames.Window_Report_QueryExcessFare
                                  ))
                           // 查询--发票查询
                           .AddItem(
                              new MenuItemDefinition(
                                  PermissionNames.Window_Report_QueryInvoiceReport,
                                  L(PermissionNames.Window_Report_QueryInvoiceReport),
                                  icon: null,
                                  url: Url("ticket.module.queryInvoiceReport", (int)ReportUrlEnum.QueryInvoiceReport, ReportUrlEnum.QueryInvoiceReport.DisplayDescription()),//"ticket.report.OrderDetail",
                                  requiresAuthentication: true,
                                  requiredPermissionName: PermissionNames.Window_Report_QueryInvoiceReport
                                  ))


                            // 查询--预订查询
                            .AddItem(
                              new MenuItemDefinition(
                                  PermissionNames.Window_Report_QueryGroupOrder,
                                  L(PermissionNames.Window_Report_QueryGroupOrder),
                                  icon: null,
                                  url: Url("ticket.module.queryGroupOrder", (int)ReportUrlEnum.QueryGroupOrder, ReportUrlEnum.QueryGroupOrder.DisplayDescription()),//"ticket.report.OrderDetail",
                                  requiresAuthentication: true,
                                  requiredPermissionName: PermissionNames.Window_Report_QueryGroupOrder
                                  ))

                            // 查询--年卡挂失解挂明细
                            .AddItem(
                              new MenuItemDefinition(
                                  PermissionNames.Window_Report_QueryICLossOp,
                                  L(PermissionNames.Window_Report_QueryICLossOp),
                                  icon: null,
                                  url: Url("ticket.module.queryICLossOp", (int)ReportUrlEnum.QueryICLossOp, ReportUrlEnum.QueryICLossOp.DisplayDescription()),//"ticket.report.OrderDetail",
                                  requiresAuthentication: true,
                                  requiredPermissionName: PermissionNames.Window_Report_QueryICLossOp
                                  ))

                           // 查询--支付交易明细查询
                           .AddItem(
                              new MenuItemDefinition(
                                  PermissionNames.Window_Report_QueryTradeInfoDetail,
                                  L(PermissionNames.Window_Report_QueryTradeInfoDetail),
                                  icon: null,
                                  url: Url("ticket.module.queryTradeInfoDetail", (int)ReportUrlEnum.QueryTradeInfoDetail, ReportUrlEnum.QueryTradeInfoDetail.DisplayDescription()),//"ticket.report.OrderDetail",
                                  requiresAuthentication: true,
                                  requiredPermissionName: PermissionNames.Window_Report_QueryTradeInfoDetail
                                  ))
                           // 查询--已售未入园查询
                           .AddItem(
                              new MenuItemDefinition(
                                  PermissionNames.Window_Report_QuerySoldNotInPark,
                                  L(PermissionNames.Window_Report_QuerySoldNotInPark),
                                  icon: null,
                                  url: Url("ticket.module.querySoldNotInPark", (int)ReportUrlEnum.QuerySoldNotInPark, ReportUrlEnum.QuerySoldNotInPark.DisplayDescription()),//"ticket.report.OrderDetail",
                                  requiresAuthentication: true,
                                  requiredPermissionName: PermissionNames.Window_Report_QuerySoldNotInPark
                                  ))

                           // 团体挂账明细
                           .AddItem(
                              new MenuItemDefinition(
                                  PermissionNames.Window_Report_GroupAccountOnDetail,
                                  L(PermissionNames.Window_Report_GroupAccountOnDetail),
                                  icon: null,
                                  url: Url("ticket.module.groupAccountOnDetail", (int)ReportUrlEnum.GroupAccountOnDetail, ReportUrlEnum.GroupAccountOnDetail.DisplayDescription()),//"ticket.report.OrderDetail",
                                  requiresAuthentication: true,
                                  requiredPermissionName: PermissionNames.Window_Report_GroupAccountOnDetail
                                  ))
                            //审计--入园人数明细表
                            .AddItem(
                                new MenuItemDefinition(
                                    PermissionNames.Window_Report_AuditInParkPersonsCount,
                                    L(PermissionNames.Window_Report_AuditInParkPersonsCount),
                                    icon: null,
                                    url: Url("ticket.module.auditInParkPersonsCount", (int)ReportUrlEnum.AuditInParkPersonsCount, ReportUrlEnum.AuditInParkPersonsCount.DisplayDescription()),
                                    requiresAuthentication: true,
                                    requiredPermissionName: PermissionNames.Window_Report_AuditInParkPersonsCount
                                    ))
                             //审计--售票明细表
                             
                              // 审计--售票汇总表
                              .AddItem(
                                new MenuItemDefinition(
                                    PermissionNames.Window_Report_AuditSaleTicketSummary,
                                    L(PermissionNames.Window_Report_AuditSaleTicketSummary),
                                    icon: null,
                                    url: Url("ticket.module.auditSaleTicketSummary", (int)ReportUrlEnum.AuditSaleTicketSummary, ReportUrlEnum.AuditSaleTicketSummary.DisplayDescription()),
                                    requiresAuthentication: true,
                                    requiredPermissionName: PermissionNames.Window_Report_AuditSaleTicketSummary
                                    ))
                        )
                .AddItem(
                    new MenuItemDefinition(
                        PermissionNames.Window_System,
                        L(PermissionNames.Window_System),
                        icon: "fa fa-cog",
                        url: "system",
                        requiresAuthentication: true,
                        requiredPermissionName: PermissionNames.Window_System
                        )

                        .AddItem(
                            new MenuItemDefinition(
                                PermissionNames.Window_System_PrintSet,
                                L(PermissionNames.Window_System_PrintSet),
                                icon: null,
                                url: "ticket.module.printerSet",
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Window_System_PrintSet
                                ))

                                .AddItem(
                            new MenuItemDefinition(
                                PermissionNames.Window_System_InvoiceCode,
                                L(PermissionNames.Window_System_InvoiceCode),
                                icon: null,
                                url: "ticket.module.invoiceCode",
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Window_System_InvoiceCode
                                ))

                                .AddItem(
                            new MenuItemDefinition(
                                PermissionNames.Window_System_PrintTemplate,
                                L(PermissionNames.Window_System_PrintTemplate),
                                icon: null,
                                url: "ticket.module.printMould",
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Window_System_PrintTemplate
                                ))

                                .AddItem(
                            new MenuItemDefinition(
                                PermissionNames.Window_System_SelfTicket,
                                L(PermissionNames.Window_System_SelfTicket),
                                icon: null,
                                url: "ticket.module.vendorSet",
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Window_System_SelfTicket
                                ))

                                .AddItem(
                            new MenuItemDefinition(
                                PermissionNames.Window_System_SystemSet,
                                L(PermissionNames.Window_System_SystemSet),
                                icon: null,
                                url: "ticket.module.systemSet",
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Window_System_SystemSet
                                ))

                                .AddItem(
                            new MenuItemDefinition(
                                PermissionNames.Window_System_EnterTimesSet,
                                L(PermissionNames.Window_System_EnterTimesSet),
                                icon: null,
                                url: "ticket.module.enterTimesSet",
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Window_System_EnterTimesSet
                                ))

                                .AddItem(
                            new MenuItemDefinition(
                                PermissionNames.Window_System_TicketTypeSet,
                                L(PermissionNames.Window_System_TicketTypeSet),
                                icon: null,
                                url: "ticket.module.ticketTypeSet",
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Window_System_TicketTypeSet
                                ))

                                .AddItem(
                            new MenuItemDefinition(
                                PermissionNames.Window_System_TerminalSet,
                                L(PermissionNames.Window_System_TerminalSet),
                                icon: null,
                                url: "ticket.module.terminal",
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Window_System_TerminalSet
                                ))

                                .AddItem(
                            new MenuItemDefinition(
                                PermissionNames.Window_System_YearCardSet,
                                L(PermissionNames.Window_System_YearCardSet),
                                icon: null,
                                url: "ticket.module.yearCardSet",
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Window_System_YearCardSet
                                ))
                       );
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, ThemeParkConsts.LocalizationSourceName);
        }

        private string Url(string stateName, int type, string title)
        {
            return stateName + "({type:" + type + ",title:'" + title + "'" + "})";
        }
    }
}