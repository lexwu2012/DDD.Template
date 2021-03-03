using Abp.BackgroundJobs;
using Abp.Configuration;
using Abp.Runtime.Caching;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ThemePark.Application.DataSync.Interfaces;
using ThemePark.Core.DataSync;
using ThemePark.Core.Settings;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.DataSync
{
    /// <summary>
    /// 中心数据同步管理 
    /// </summary>
    public class CenterDataSyncManager : DataSyncManager, ICenterDataSyncManager
    {
        #region Fields
        private readonly int _localParkId;//本地(服务部署)公园Id
        private readonly SyncPark _localPark;//本地(服务部署)公园信息
        private readonly IList<SyncPark> _syncParks;//需要进行数据同步的公园
        private readonly string _latestTableDataUploadTimeCacheKey;//最后一次表数据上传时间缓存Key

        private readonly ISyncParkAppService _syncParkAppService;
        #endregion

        /// <summary>
        /// 最后一次表数据上传时间
        /// </summary>
        public DateTime LatestTableDataUploadTime
        {
            get
            {
                return base.DataSyncCache.Get(this._latestTableDataUploadTimeCacheKey, () =>
                {
                    return DateTime.Now.Date.Add(new TimeSpan(this._localPark.UploadTime.Hour, this._localPark.UploadTime.Minute, this._localPark.UploadTime.Second));
                });
            }
            private set
            {
                base.DataSyncCache.Set(this._latestTableDataUploadTimeCacheKey, value);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CenterDataSyncManager(
            ICacheManager cacheManager,
            ISettingManager settingManager,
            ISyncParkAppService syncParkAppService,
            IDataSyncAppService dataSyncAppService,
            IBackgroundJobManager backgroundJobManager)
            : base(cacheManager, settingManager, dataSyncAppService, backgroundJobManager)
        {
            this._syncParkAppService = syncParkAppService;

            this._localParkId = base.SettingManager.GetSettingValueForApplication<int>(DataSyncSetting.LocalParkId);
            this._syncParks = this._syncParkAppService.GetAllList().Where(k => k.IsActive && !k.IsDeleted).ToList();//获取所有有效的需要进行数据同步公园
            this._localPark = this._syncParks.FirstOrDefault(k => k.ParkId == this._localParkId);
            this._latestTableDataUploadTimeCacheKey = $"{this._localParkId:D5}_latestTableDataUploadTime";
        }


        /// <summary>
        /// 向公园上传表数据(修改/新增时间介于最后一次上传时间->当前时间的表数据)
        /// </summary>
        /// <param name="currentTime">当前时间</param>
        public async Task<Result> UploadTableDataToPark(DateTime currentTime)
        {
            var ss = LatestTableDataUploadTime;
            var result = await base.DataSyncAppService.UploadTableUpdateFile(LatestTableDataUploadTime, currentTime);
            if (!result.Success)
                throw new Exception(result.Message);
            else if (!string.IsNullOrWhiteSpace(result.Message))//如果返回有数据更新文件名，则通知公园下载更新文件进行表数据更新
            {
                var targetParks = this._syncParks.Where(k => k.ParkId != this._localParkId && k.IsActive).ToList();
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

                        base.BackgroundJobManager.Enqueue<ApiDataSyncJob, ApiDataSyncJobArgs>(apiDataSyncJobArgs);
                    });
                }
                this.LatestTableDataUploadTime = currentTime;
            }

            return Result.Ok();
        }

        /// <summary>
        /// 向公园上传表数据(修改/新增时间介于开始时间->结束时间的表数据)
        /// </summary>
        /// <param name="timeFrom">开始时间</param>
        /// <param name="timeTo">结束时间</param>
        /// <returns></returns>
        public async Task<Result> UploadTableDataToPark(DateTime timeFrom, DateTime timeTo)
        {
            var result = await base.DataSyncAppService.UploadTableUpdateFile(timeFrom, timeTo);
            if (!result.Success)
                throw new Exception(result.Message);
            else if (!string.IsNullOrWhiteSpace(result.Message))//如果返回有数据更新文件名，则通知公园下载更新文件进行表数据更新
            {
                var targetParks = this._syncParks.Where(k => k.ParkId != this._localParkId && k.IsActive).ToList();
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

                        base.BackgroundJobManager.Enqueue<ApiDataSyncJob, ApiDataSyncJobArgs>(apiDataSyncJobArgs);
                    });
                }
            }

            return Result.Ok();
        }

        /// <summary>
        /// 向特定公园上传表数据(修改/新增时间介于开始时间->结束时间的表数据)
        /// </summary>
        /// <param name="targetParkIds"></param>
        /// <param name="timeFrom">开始时间</param>
        /// <param name="timeTo">结束时间</param>
        /// <returns></returns>
        public async Task<Result> UploadTableDataToTargetPark(List<int> targetParkIds, DateTime timeFrom, DateTime timeTo)
        {
            var result = await base.DataSyncAppService.UploadTableUpdateFile(timeFrom, timeTo);
            if (!result.Success)
                throw new Exception(result.Message);
            else if (!string.IsNullOrWhiteSpace(result.Message))//如果返回有数据更新文件名，则通知公园下载更新文件进行表数据更新
            {
                var targetParks = this._syncParks.Where(k => targetParkIds.Contains(k.ParkId) && k.ParkId != this._localParkId && k.IsActive).ToList();
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

                        base.BackgroundJobManager.Enqueue<ApiDataSyncJob, ApiDataSyncJobArgs>(apiDataSyncJobArgs);
                    });
                }
            }

            return Result.Ok();
        }

        /// <summary>
        /// 启动文件监视以下载公园同步上来的表数据
        /// </summary>
        public void InitFileWatchForDownloadTableData()
        {
            // 监控上传文件
            var fileWatcher = new FileSystemWatcher
            {
                Path = base.SettingManager.GetSettingValueForApplication(DataSyncSetting.UploadFilePath),
                Filter = "*.*"
            };

            fileWatcher.Renamed += OnFileRenamed;
            fileWatcher.EnableRaisingEvents = true;
            fileWatcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite;
            fileWatcher.IncludeSubdirectories = false;
        }

        /// <summary>
        /// 当监测到文件名称变化时
        /// </summary>
        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            var centerTableDataDownloadArgs = new CenterTableDataDownloadArgs
            {
                DataFileFullPath = e.FullPath
            };

            base.BackgroundJobManager.Enqueue<CenterTableDataDownloadJob, CenterTableDataDownloadArgs>(centerTableDataDownloadArgs);
        }
    }
}
