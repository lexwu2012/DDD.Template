using Abp.Application.Services;
using ThemePark.Application.DataSync.Dto;

namespace ThemePark.Application.OTA
{
    /// <summary>
    /// OTA订单同步
    /// </summary>
    public interface IOTADataSync: IApplicationService
    {
        /// <summary>
        /// 同步订单
        /// </summary>
        /// <returns></returns>
        void SynOrderAsync(OrderSendDto data,int parkId);

        /// <summary>
        /// 同步修改数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="parkId"></param>
        void SynModifyDataAsync(OrderModifyDto data, int parkId);
    }
}
