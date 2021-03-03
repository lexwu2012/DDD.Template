using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Events.Bus;
using ThemePark.Application.DataSync.Interfaces;
using ThemePark.Core.DataSync;

namespace ThemePark.Application.DataSync
{
    /// <summary>
    /// 中心表数据下载Job
    /// </summary>
    public class CenterTableDataDownloadJob : BackgroundJob<CenterTableDataDownloadArgs>, ITransientDependency
    {
        private readonly IDataSyncAppService _dataSyncAppService;

        /// <summary>
        /// .Ctor
        /// </summary>
        public CenterTableDataDownloadJob(IDataSyncAppService dataSyncAppService)
        {
            this._dataSyncAppService = dataSyncAppService;
        }

        /// <summary>
        /// 表数据下载
        /// </summary>
        public override void Execute(CenterTableDataDownloadArgs args)
        {
            if (!string.IsNullOrWhiteSpace(args.DataFileFullPath))
            {
                var result = this._dataSyncAppService.UpdateTables(args.DataFileFullPath);
                if (!result.Success)
                    throw new System.Exception(result.Message);
                else//触发清除缓存事件
                {
                    EventBus.Default.Trigger(new DataSyncEventData(DataSyncType.UpdateTables));
                }
            }
        }
    }

    /// <summary>
    /// 中心表数据下载参数(文件生成表数据)
    /// </summary>
    public class CenterTableDataDownloadArgs
    {
        /// <summary>
        /// 数据文件全路径
        /// </summary>
        public string DataFileFullPath { get; set; }

    }
}
