using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using ThemePark.Application.SaleTicekt.Dto;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.SaleTicekt.Interfaces
{
    /// <summary>
    /// 门票打印接口
    /// </summary>
    public interface IPrintTicketAppService: IApplicationService
    {
        /// <summary>
        /// 取打印内容
        /// </summary>
        /// <returns></returns>
        Task<Result<List<PrintInfo>>> GetPrintContentTask(PrintTicketType type, string tradeno = null, string barcode = null);
    }



    /// <summary>
    /// 打印类型
    /// </summary>
    public enum PrintTicketType
    {
        
        /// <summary>
        /// 打印散客票
        /// </summary>
        NonGroupTicket = 0,

        /// <summary>
        /// 打印团体票
        /// </summary>
        GroupTicket = 5,

        /// <summary>
        /// 打印电商票/旅行社票
        /// </summary>
        WebTicket = 10,

        /// <summary>
        /// 打印入园单
        /// </summary>
        Admission = 15,

        /// <summary>
        /// 打印补票
        /// </summary>
        FareAdjustment = 30,

        /// <summary>
        /// 重打印
        /// </summary>
        RePrint = 40,

        /// <summary>
        /// 年卡打印
        /// </summary>
        YearCard = 50,

        /// <summary>
        /// 年卡凭证打印
        /// </summary>
        YearCardVoucher = 60,

        /// <summary>
        /// 他园票
        /// </summary>
        OtherNonGroupTicket = 70,
    }
}
