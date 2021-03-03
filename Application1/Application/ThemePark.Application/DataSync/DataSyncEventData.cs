using Abp.Events.Bus;
using ThemePark.Core.DataSync;

namespace ThemePark.Application.DataSync
{
    public class DataSyncEventData : EventData
    {
        public DataSyncType DataSyncType { get; set; }

        /// <summary>Constructor.</summary>
        public DataSyncEventData(DataSyncType dataSyncType)
        {
            DataSyncType = dataSyncType;
        }
    }
}
