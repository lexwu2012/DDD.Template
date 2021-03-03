using Abp.Application.Navigation;
using Abp.Localization;
using ThemePark.Core;
using ThemePark.Core.Authorization;
using ThemePark.Infrastructure.Enumeration;

namespace ThemePark.Navigation
{
    /// <summary>
    /// This class defines menus for the application.
    /// It uses ABP's menu system.
    /// When you add menu items here, they are automatically appear in angular application.
    /// See Views/Layout/_TopMenu.cshtml file to know how to render menu.
    /// </summary>
    public class ThemeParkWebNavigationProvider : NavigationProvider
    {
        public const string MenuName = "MainMenu";

        public override void SetNavigation(INavigationProviderContext context)
        {
            context.Manager.MainMenu
                .AddItem(
                    new MenuItemDefinition(
                        PermissionNames.Pages_Basic,
                        L(PermissionNames.Pages_Basic),
                        icon: "icon-basic_data",
                        url: "",
                        requiresAuthentication: true,
                        requiredPermissionName: PermissionNames.Pages_Basic
                        )
                    {
                        Items =
                        {
                            new MenuItemDefinition(
                                PermissionNames.Pages_Basic_Park,
                                L(PermissionNames.Pages_Basic_Park),
                                icon: "",
                                url: Url("Index", "Park"),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Basic_Park
                                ),
                            new MenuItemDefinition(
                                PermissionNames.Pages_Basic_BasicTicket,
                                L(PermissionNames.Pages_Basic_BasicTicket),
                                icon:"",
                                url: Url("Index", "TicketType"),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Basic_BasicTicket
                                ),
                            new MenuItemDefinition(
                                PermissionNames.Pages_Basic_InParkRule,
                                L(PermissionNames.Pages_Basic_InParkRule),
                                icon:"",
                                url: Url("Index", "InParkRule"),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Basic_InParkRule
                                ),
                            new MenuItemDefinition(
                                PermissionNames.Pages_Basic_TicketClass,
                                L(PermissionNames.Pages_Basic_TicketClass),
                                icon:"",
                                url: Url("Index", "TicketClass"),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Basic_TicketClass
                                ),
                            new MenuItemDefinition(
                                PermissionNames.Pages_Basic_SaleTicketClass,
                                L(PermissionNames.Pages_Basic_SaleTicketClass),
                                icon:"",
                                url: Url("Index", "ParkSaleTicketClass"),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Basic_SaleTicketClass
                                )
                        }
                    }
                 ).AddItem(
                    new MenuItemDefinition(
                        PermissionNames.Pages_Agency,
                        L(PermissionNames.Pages_Agency),
                        icon: "icon-system_user",
                        url: "",
                        requiresAuthentication: true,
                        requiredPermissionName: PermissionNames.Pages_Agency
                        )
                    {
                        Items =
                        {
                            new MenuItemDefinition(
                                PermissionNames.Pages_Agency_GroupType,
                                L(PermissionNames.Pages_Agency_GroupType),
                                icon:"",
                                url: Url("Index", "GroupType"),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Agency_GroupType
                                ),
                            new MenuItemDefinition(
                                PermissionNames.Pages_Agency_GroupTypeTicketClass,
                                L(PermissionNames.Pages_Agency_GroupTypeTicketClass),
                                icon:"",
                                url: Url("Index", "GroupTypeTicketClass"),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Agency_GroupTypeTicketClass
                                ),
                           new MenuItemDefinition(
                                PermissionNames.Pages_Agency_AgencyType,
                                L(PermissionNames.Pages_Agency_AgencyType),
                                icon:"",
                                url: Url("Index", "AgencyType"),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Agency_AgencyType
                                ),
                           new MenuItemDefinition(
                                PermissionNames.Pages_Agency_AgencyRule,
                                L(PermissionNames.Pages_Agency_AgencyRule),
                                icon:"",
                                url: Url("Index", "AgencyRule"),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Agency_AgencyRule
                                ),
                           new MenuItemDefinition(
                                PermissionNames.Pages_Agency_Info,
                                L(PermissionNames.Pages_Agency_Info),
                                icon:"",
                                url: Url("Index", "Agency"),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Agency_Info
                                ),
                           new MenuItemDefinition(
                                PermissionNames.Pages_Agency_ParkAgencyTypeGroupType,
                                L(PermissionNames.Pages_Agency_ParkAgencyTypeGroupType),
                                icon:"",
                                url: Url("Index", "ParkAgencyTypeGroupType"),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Agency_ParkAgencyTypeGroupType
                                ),
                           //new MenuItemDefinition(
                           //     PermissionNames.Pages_Agency_ParkAgency,
                           //     L(PermissionNames.Pages_Agency_ParkAgency),
                           //     icon:"",
                           //     url: Url("Index", "ParkAgency"),
                           //     requiresAuthentication: true,
                           //     requiredPermissionName: PermissionNames.Pages_Agency_ParkAgency
                           //     ),
                           new MenuItemDefinition(
                                PermissionNames.Pages_Agency_SaleTicketTemp,
                                L(PermissionNames.Pages_Agency_SaleTicketTemp),
                                icon:"",
                                url: Url("Index", "AgencySaleTicketClassTemplate"),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Agency_SaleTicketTemp
                                ),
                           new MenuItemDefinition(
                                PermissionNames.Pages_Agency_SaleTicket,
                                L(PermissionNames.Pages_Agency_SaleTicket),
                                icon:"",
                                url: Url("Index", "AgencySaleTicketClass"),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Agency_SaleTicket
                                ),
                            new MenuItemDefinition(
                                PermissionNames.Pages_Agency_PreAccount,
                                L(PermissionNames.Pages_Agency_PreAccount),
                                icon:"",
                                url: Url("Index", "PreAccount"),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Agency_PreAccount
                                ),
                             new MenuItemDefinition(
                                PermissionNames.Pages_Agency_PreAccountOp,
                                L(PermissionNames.Pages_Agency_PreAccountOp),
                                icon:"",
                                url: Url("Index", "PreAccountOp"),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Agency_PreAccountOp
                                )
                        },
                    }
                ).AddItem(
                    new MenuItemDefinition(
                        PermissionNames.Pages_Order,
                        L(PermissionNames.Pages_Order),
                        icon: "icon-web_book",
                        url: "",
                        requiresAuthentication: true,
                        requiredPermissionName: PermissionNames.Pages_Order
                        )
                    {
                        Items =
                        {
                            new MenuItemDefinition(
                                PermissionNames.Pages_Order_Online,
                                L(PermissionNames.Pages_Order_Online),
                                icon: "",
                                url: Url("Index","OnlineAgencyOrder"),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Order_Online
                                ),
                            new MenuItemDefinition(
                                PermissionNames.Pages_Order_OTAReserveRegister,
                                L(PermissionNames.Pages_Order_OTAReserveRegister),
                                icon: "",
                                url: Url("Index","ReserveRegister"),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Order_OTAReserveRegister
                                ),
                           new MenuItemDefinition(
                                PermissionNames.Pages_Order_OTAOnline,
                                L(PermissionNames.Pages_Order_OTAOnline),
                                icon:"",
                                url: Url("Index","OTAOrder"),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Order_OTAOnline
                                )
                        }
                    }
                ).AddItem(
                    new MenuItemDefinition(
                        PermissionNames.Pages_Notice,
                        L(PermissionNames.Pages_Notice),
                        icon: "icon-notice",
                        url: "",
                        requiresAuthentication: true,
                        requiredPermissionName: PermissionNames.Pages_Notice
                        )
                    {
                        Items =
                        {
                            new MenuItemDefinition(
                                PermissionNames.Pages_Notice_NoticeType,
                                L(PermissionNames.Pages_Notice_NoticeType),
                                icon:"",
                                url: Url("Index", "NoticeType"),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Notice_NoticeType
                            ),
                            new MenuItemDefinition(
                                PermissionNames.Pages_Notice_Index,
                                L(PermissionNames.Pages_Notice_Index),
                                icon:"",
                                url: Url("Index", "Notice"),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Notice_Index
                            )
                        }
                    }
                ).AddItem(
                    new MenuItemDefinition(
                        PermissionNames.Pages_Permissions,
                        L(PermissionNames.Pages_Permissions),
                        icon: "icon-system_right",
                        url: "",
                        requiresAuthentication: true,
                        requiredPermissionName: PermissionNames.Pages_Permissions
                        )
                    {
                        Items =
                        {
                            new MenuItemDefinition(
                                PermissionNames.Pages_Permissions_Index,
                                L(PermissionNames.Pages_Permissions_Index),
                                icon:"",
                                url:Url("Index", "Permission"),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Permissions_Index
                                ),
                            new MenuItemDefinition(
                                PermissionNames.Pages_Permissions_Roles,
                                L(PermissionNames.Pages_Permissions_Roles),
                                icon:"",
                                url:Url("RolesIndex", "Permission"),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Permissions_Roles
                                ),
                            new MenuItemDefinition(
                                PermissionNames.Pages_Permissions_Data,
                                L(PermissionNames.Pages_Permissions_Data),
                                icon:"",
                                url: Url("DataPermissions", "Permission"),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Permissions_Data
                                )
                        }
                    }
                ).AddItem(
                    new MenuItemDefinition(
                        PermissionNames.Pages_Users,
                        L(PermissionNames.Pages_Users),
                        icon: "icon-user_manage",
                        url: "",
                        requiresAuthentication: true,
                        requiredPermissionName: PermissionNames.Pages_Users
                        )
                    {
                        Items =
                        {
                            new MenuItemDefinition(
                                PermissionNames.Pages_Users_Index,
                                L(PermissionNames.Pages_Users_Index),
                                icon:"",
                                url:Url("SysUsers", "Users"),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Users_Index
                                ),
                            //new MenuItemDefinition(
                            //    PermissionNames.Pages_Users_Agency,
                            //    L(PermissionNames.Pages_Users_Agency),
                            //    icon:"",
                            //    url:Url("AgencyUsers", "Users"),
                            //    requiresAuthentication: true,
                            //    requiredPermissionName: PermissionNames.Pages_Users_Agency
                            //    ),
                            //new MenuItemDefinition(
                            //    PermissionNames.Pages_Users_Api,
                            //    L(PermissionNames.Pages_Users_Api),
                            //    icon:"",
                            //    url:Url("ApiUsers", "Users"),
                            //    requiresAuthentication: true,
                            //    requiredPermissionName: PermissionNames.Pages_Users_Api
                            //    ),
                            new MenuItemDefinition(
                                PermissionNames.Pages_Users_ResetPwd,
                                L(PermissionNames.Pages_Users_ResetPwd),
                                icon:"",
                                url:Url("AllUsers", "Users"),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Users_ResetPwd
                                )
                        }
                    }
                ).AddItem(
                    new MenuItemDefinition(
                        PermissionNames.Pages_Report,
                        L(PermissionNames.Pages_Report),
                        icon: "icon-form",
                        url: "",
                        requiresAuthentication: true,
                        requiredPermissionName: PermissionNames.Pages_Report
                        )
                    {
                        Items =
                        {
                            //测试报表
                           /* new MenuItemDefinition(
                                PermissionNames.Pages_Report_TestReport,
                                L(PermissionNames.Pages_Report_TestReport),
                                icon:"",
                                url:Url("Report","Report",(int)ReportUrlEnum. TestReport),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Report_TestReport
                                ),*/
                             //OTA--官方总体查询报表 200
                            new MenuItemDefinition(
                                PermissionNames.Pages_Report_OTAOfficialSummary,
                                L(PermissionNames.Pages_Report_OTAOfficialSummary),
                                icon:"",
                                url:Url("Report","Report",(int)ReportUrlEnum. OTAOfficialSummary),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Report_OTAOfficialSummary
                                ),
                            // OTA--官方订单明细 205
                            new MenuItemDefinition(
                                PermissionNames.Pages_Report_OTAOfficialDetail,
                                L(PermissionNames.Pages_Report_OTAOfficialDetail),
                                icon:"",
                                url:Url("Report","Report",(int)ReportUrlEnum.OTAOfficialDetail),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Report_OTAOfficialDetail
                                ),
                             // OTA--旅游网总体查询 220
                            new MenuItemDefinition(
                                PermissionNames.Pages_Report_OTAOnlineSummary,
                                L(PermissionNames.Pages_Report_OTAOnlineSummary),
                                icon:"",
                                url:Url("Report","Report",(int)ReportUrlEnum.OTAOnlineSummary),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Report_OTAOnlineSummary
                                ),
                            // OTA--旅游网订单明细 225
                            new MenuItemDefinition(
                                PermissionNames.Pages_Report_OTAOnlineDetail,
                                L(PermissionNames.Pages_Report_OTAOnlineDetail),
                                icon:"",
                                url:Url("Report","Report",(int)ReportUrlEnum.OTAOnlineDetail),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Report_OTAOnlineDetail
                                ),
                            // OTA--各渠道网络销售汇总 230
                            new MenuItemDefinition(
                                PermissionNames.Pages_Report_OTASingleSource,
                                L(PermissionNames.Pages_Report_OTASingleSource),
                                icon:"",
                                url:Url("Report","Report",(int)ReportUrlEnum.OTASingleSource),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Report_OTASingleSource
                                ),
                            // OTA--各渠道网络销售汇总(按日期) 240
                            new MenuItemDefinition(
                                PermissionNames.Pages_Report_OTASingleSourceByDate,
                                L(PermissionNames.Pages_Report_OTASingleSourceByDate),
                                icon:"",
                                url:Url("Report","Report",(int)ReportUrlEnum. OTASingleSourceByDate),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Report_OTASingleSourceByDate
                                ),
                            // 七、OTA--各公园网络销售汇总 250
                            new MenuItemDefinition(
                                PermissionNames.Pages_Report_OTAAllParkSummary,
                                L(PermissionNames.Pages_Report_OTAAllParkSummary),
                                icon:"",
                                url:Url("Report","Report",(int)ReportUrlEnum.OTAAllParkSummary),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Report_OTAAllParkSummary
                                ),
                            // 八、OTA--单公园网络销售汇总 260
                            new MenuItemDefinition(
                                PermissionNames.Pages_Report_OTASingleParkSummary,
                                L(PermissionNames.Pages_Report_OTASingleParkSummary),
                                icon:"",
                                url:Url("Report","Report",(int)ReportUrlEnum.OTASingleParkSummary),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Report_OTASingleParkSummary
                                ),
                            // 九、OTA--单个产品网络销售统计 270
                            new MenuItemDefinition(
                                PermissionNames.Pages_Report_OTASingleProduct,
                                L(PermissionNames.Pages_Report_OTASingleProduct),
                                icon:"",
                                url:Url("Report","Report",(int)ReportUrlEnum.OTASingleProduct),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Report_OTASingleProduct
                                ),
                            //旅行社综合报表
                            new MenuItemDefinition(
                                PermissionNames.Pages_Report_TravelAgencyIntegrateReport,
                                L(PermissionNames.Pages_Report_TravelAgencyIntegrateReport),
                                icon:"",
                                url:Url("Report","Report",(int)ReportUrlEnum. TravelAgencyIntegrateReport),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Report_TravelAgencyIntegrateReport
                                ),
                            //旅行社日报表汇总
                            new MenuItemDefinition(
                                PermissionNames.Pages_Report_TravelAgencyMonthReport,
                                L(PermissionNames.Pages_Report_TravelAgencyMonthReport),
                                icon:"",
                                url:Url("Report","Report",(int)ReportUrlEnum. TravelAgencyMonthReport),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Report_TravelAgencyMonthReport
                                ),
                            //旅行社订单明细
                            new MenuItemDefinition(
                                PermissionNames.Pages_Report_TravelAgencyOrderDetail,
                                L(PermissionNames.Pages_Report_TravelAgencyOrderDetail),
                                icon:"",
                                url:Url("Report","Report",(int)ReportUrlEnum. TravelAgencyOrderDetail),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Report_TravelAgencyOrderDetail
                                ),
                             //OTA促销票类信息
                            new MenuItemDefinition(
                                PermissionNames.Pages_Report_OTASaleTicketClassInfo,
                                L(PermissionNames.Pages_Report_OTASaleTicketClassInfo),
                                icon:"",
                                url:Url("Report","Report",(int)ReportUrlEnum. OTASaleTicketClassInfo),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Report_OTASaleTicketClassInfo
                                ),
                             //OTA订单手机号管理
                            new MenuItemDefinition(
                                PermissionNames.Pages_Report_OTAOrderPhoneNumber,
                                L(PermissionNames.Pages_Report_OTAOrderPhoneNumber),
                                icon:"",
                                url:Url("Report","Report",(int)ReportUrlEnum. OTAOrderPhoneNumber),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Report_OTAOrderPhoneNumber
                                ),
                             //Summary--总体销售分析 330
                            new MenuItemDefinition(
                                PermissionNames.Pages_Report_SummarySalesAnalysis,
                                L(PermissionNames.Pages_Report_SummarySalesAnalysis),
                                icon:"",
                                url:Url("Report","Report",(int)ReportUrlEnum. SummarySalesAnalysis),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Report_SummarySalesAnalysis
                                ),
                             //Summary--公园销售分析 340
                            new MenuItemDefinition(
                                PermissionNames.Pages_Report_SummarySalesAnalysisOnePark,
                                L(PermissionNames.Pages_Report_SummarySalesAnalysisOnePark),
                                icon:"",
                                url:Url("Report","Report",(int)ReportUrlEnum. SummarySalesAnalysisOnePark),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Report_SummarySalesAnalysisOnePark
                                ),
                        }
                    }
                ).AddItem(
                    new MenuItemDefinition(
                        PermissionNames.Pages_Data,
                        L(PermissionNames.Pages_Data),
                        icon: "icon-data_synchro",
                        url: "",
                        requiresAuthentication: true,
                        requiredPermissionName: PermissionNames.Pages_Data
                        )
                    {
                        Items =
                        {
                            new MenuItemDefinition(
                                PermissionNames.Pages_Data_ParkInfo,
                                L(PermissionNames.Pages_Data_ParkInfo),
                                icon:"",
                                url:Url("Index","SyncPark"),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Data_ParkInfo
                                ),
                            new MenuItemDefinition(
                                PermissionNames.Pages_Data_DataTable,
                                L(PermissionNames.Pages_Data_DataTable),
                                icon:"",
                                url:Url("Index","SyncTable"),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Data_DataTable
                                ),
                            new MenuItemDefinition(
                                PermissionNames.Pages_Data_DataTransferLog,
                                L(PermissionNames.Pages_Data_DataTransferLog),
                                icon:"",
                                url:Url("Index","SyncLog"),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Data_DataTransferLog
                                ),
                            new MenuItemDefinition(
                                PermissionNames.Pages_Data_FtpConfiguration,
                                L(PermissionNames.Pages_Data_FtpConfiguration),
                                icon:"",
                                url:Url("Index","FtpConfiguration"),
                                requiresAuthentication: true,
                                requiredPermissionName: PermissionNames.Pages_Data_FtpConfiguration
                                )
                        }
                    }
                );
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, ThemeParkConsts.LocalizationSourceName);
        }

        private string Url(string actionName, string controllerName)
        {
            //can't get the RequestContext, because the Provider is executed at PostInitialize
            //return new UrlHelper(HttpContext.Current.Request.RequestContext).Action(actionName, controllerName);

            return actionName + "_" + controllerName;
        }

        private string Url(string actionName, string controllerName, int param)
        {
            //can't get the RequestContext, because the Provider is executed at PostInitialize
            //return new UrlHelper(HttpContext.Current.Request.RequestContext).Action(actionName, controllerName);

            return actionName + "_" + controllerName + "_" + param;
        }
    }
}
