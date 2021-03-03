using Abp.BackgroundJobs;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Threading;
using System;
using System.Linq;
using ThemePark.Application.DataSync.Interfaces;
using ThemePark.Core.DataSync;
using ThemePark.Core.Settings;

namespace ThemePark.Application.DataSync
{
    /// <summary>
    /// 中心表数据上传参数
    /// </summary>
    public class CenterTableDataUploadArgs
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime TimeFrom { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime TimeTo { get; set; }
    }

    /// <summary>
    /// 中心表数据上传Job
    /// </summary>
    public class CenterTableDataUploadJob : BackgroundJob<CenterTableDataUploadArgs>, ITransientDependency
    {
        private readonly IDataSyncAppService _dataSyncAppService;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly ISyncParkAppService _syncParkAppService;

        /// <summary>
        /// 构造函数
        /// </summary>
        public CenterTableDataUploadJob(IDataSyncAppService dataSyncAppService, ISyncParkAppService syncParkAppService, IBackgroundJobManager backgroundJobManager)
        {
            this._dataSyncAppService = dataSyncAppService;
            this._syncParkAppService = syncParkAppService;
            this._backgroundJobManager = backgroundJobManager;
        }

        /// <summary>
        /// 表数据上传
        /// </summary>
        public override void Execute(CenterTableDataUploadArgs args)
        {
            var localParkId = SettingManager.GetSettingValueForApplication<int>(DataSyncSetting.LocalParkId);
            //if (localParkId == ThemeParkConsts.CenterParkId)
            //{
            var result = AsyncHelper.RunSync(() => this._dataSyncAppService.UploadTableUpdateFile(args.TimeFrom, args.TimeTo));
            if (result.Success)//如果表数据更新文件上传成功，通知公园下载更新文件进行表数据更新
            {
                var targetParks = this._syncParkAppService.GetAllList().Where(k => k.ParkId != localParkId && k.IsActive).ToList();
                if (targetParks.Any())
                {
                    targetParks.ForEach(x =>
                    {
                        var apiDataSyncJobArgs = new ApiDataSyncJobArgs
                        {
                            TargetParkId = x.ParkId,
                            SyncType = DataSyncType.DownloadTablesUpdateFile,
                            SyncData = result.Message//文件名
                        };

                        this._backgroundJobManager.Enqueue<ApiDataSyncJob, ApiDataSyncJobArgs>(apiDataSyncJobArgs);
                    });
                }
                //}
            }
        }

    }
}
