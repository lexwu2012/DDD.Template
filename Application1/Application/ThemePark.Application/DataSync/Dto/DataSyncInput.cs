using System;
using System.Configuration;
using ThemePark.Core.DataSync;
using ThemePark.Core.Settings;

namespace ThemePark.Application.DataSync.Dto
{
    /// <summary>
    /// 数据同步API接口数据结构
    /// </summary>
    public class DataSyncInput
    {
        /// <summary>
        /// cotr
        /// </summary>
        public DataSyncInput()
        {
            TaskId = Guid.NewGuid();
            CreationTime = DateTime.Now;
            FromPark = ConfigurationManager.AppSettings[AppConfigSetting.LocalParkId];
        }

        /// <summary>
        /// Gets or sets the task identifier.
        /// </summary>
        /// <value>The task identifier.</value>
        public Guid TaskId { get; set; }

        /// <summary>
        /// 同步类型编号
        /// </summary>
        public DataSyncType SyncType { get; set; }

        /// <summary>
        /// 同步数据json
        /// </summary>
        public string SyncData { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        public string Sign { get; set; }

        /// <summary>
        /// Gets or sets the creation time.
        /// </summary>
        /// <value>The creation time.</value>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Gets or sets from park.
        /// </summary>
        /// <value>From park.</value>
        public string FromPark { get; set; }
    }
}
