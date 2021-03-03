using Abp.Auditing;
using Newtonsoft.Json;
using ThemePark.Application.DataSync.Dto;
using ThemePark.Application.DataSync.Interfaces;
using ThemePark.Core.DataSync;

namespace ThemePark.Application.OTA
{
    [DisableAuditing]
    public class OTADataSync : IOTADataSync
    {

        private readonly IDataSyncManager _dataSyncManager;

        public OTADataSync(IDataSyncManager dataSyncManager)
        {
            _dataSyncManager = dataSyncManager;
        }

        /// <summary>
        /// 同步OTA订单信息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        public void SynOrderAsync(OrderSendDto data, int parkId)
        {
            DataSyncInput input = new DataSyncInput()
            {
                SyncData = JsonConvert.SerializeObject(data),
                SyncType = DataSyncType.OrderSend
            };
            _dataSyncManager.UploadDataToTargetPark(parkId, input);
        }

        /// <summary>
        /// 同步OTA修改数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="parkId"></param>
        public void SynModifyDataAsync(OrderModifyDto data, int parkId)
        {
            DataSyncInput input = new DataSyncInput()
            {
                SyncData = JsonConvert.SerializeObject(data),
                SyncType = DataSyncType.OrderModify
            };
            _dataSyncManager.UploadDataToTargetPark(parkId, input);
        }        
    }
}
