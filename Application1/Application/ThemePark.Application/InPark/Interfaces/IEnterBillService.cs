using Abp.Application.Services;
using System.Threading.Tasks;
using ThemePark.Application.InPark.Dto;
using ThemePark.Core.InPark;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.InPark.Interfaces
{
    /// <summary>
    /// 入园单相关接口
    /// </summary>
    public interface IEnterBillService : IApplicationService
    {
        /// <summary>
        /// 入园凭证保存
        /// </summary>
        /// <param name="enterBillInput"></param>
        /// <param name="parkId"></param>
        /// <param name="terminalId"></param>
        /// <returns></returns>
        Task<Result<string>> SaveEnterBill(EnterBillInput enterBillInput, int parkId, int terminalId);

        /// <summary>
        /// 获取最大入园单编号
        /// </summary>
        /// <returns></returns>
        Task<Result<string>> GetMaxBillNo();

        /// <summary>
        /// 分页查询入园单信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<PageResult<InParkBillDto>> GetAllByPageAsync(SearchInParkBillModel query = null);

        /// <summary>
        /// 根据ID获取入园单详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Result<InParkBillDto>> GetInParkBillInfoByIdAsync(string id);

        /// <summary>
        /// 入园单作废
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Result> DeleteInParkBillAsync(string id);

        /// <summary>
        /// 获取默认配置
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        string GetEnterBillSettingValue(string name, int parkId);

        /// <summary>
        /// 根据条码获取入园单信息
        /// </summary>
        /// <param name="barcode">The barcode.</param>
        /// <returns>Task&lt;</returns>
        Task<InParkBill> GetInParkBillByBarcodeAsync(string barcode);

        /// <summary>
        /// 根据条码确定门票是否未使用
        /// </summary>
        /// <param name="barcode">The barcode.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> CheckTicketUnusedAsync(string barcode);

        /// <summary>
        /// 手动入园
        /// </summary>
        /// <param name="barcode">The barcode.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        Task<Result> ManualInPark(string barcode);
    }
}
