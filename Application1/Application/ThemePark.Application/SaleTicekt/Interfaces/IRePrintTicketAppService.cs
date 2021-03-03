using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThemePark.Application.SaleTicekt.Dto;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.SaleTicekt.Interfaces
{
    /// <summary>
    /// 重打印门票服务接口
    /// </summary>
    public interface IRePrintTicketAppService : IApplicationService
    {
        /// <summary>
        /// 根据条码获取有效的票(或默认取最后一笔订单的所有票)
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="terminalId"></param>
        /// <param name="localParkId"></param>
        /// <returns>列表数据</returns>
        Task<Result<List<ReprintTicketPageRecord>>> GetTicketByBarCodeAsync(string barcode, int terminalId, int localParkId);

        /// <summary>
        /// 生成重打印记录
        /// </summary>
        /// <param name="model"></param>
        /// <param name="terminalId"></param>
        /// <returns></returns>
        Task<Result> GenerateReprintRecord(RePrintTicketModel model, int terminalId);
    }
}
