using Abp.BackgroundJobs;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Runtime.Caching;
using Castle.Core.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ThemePark.Application.DataSync.Interfaces;
using ThemePark.Core;
using ThemePark.Core.DataSync;
using ThemePark.Core.Settings;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.DataSync
{
    /// <summary>
    /// 中心数据同步服务
    /// </summary>
    public class CenterDataSyncService : ICenterDataSyncService, ISingletonDependency
    {
        #region Fields
        private string _syncSecretKey { get; set; }//数据同步密钥

        private int _localParkId { get; set; }//本地(服务部署)公园Id
        private SyncPark _localPark;//本地(服务部署)公园信息
        private IList<SyncPark> _syncParks { get; set; }//需要进行数据同步的公园
        private string _latestTableDataUploadTimeCacheKey;//最后一次表数据上传时间缓存Key

        public ILogger Logger { get; set; }//日志管理器
        private readonly ICache _dataSyncCache;
        private readonly ICacheManager _cacheManager;
        private readonly ISettingManager _settingManager;
        private readonly ISyncParkAppService _syncParkAppService;
        private readonly IDataSyncAppService _dataSyncAppService;
        private readonly IBackgroundJobManager _backgroundJobManager;
        #endregion

        /// <summary>
        /// 最后一次表数据上传时间
        /// </summary>
        public DateTime LatestTableDataUploadTime
        {
            get
            {
                return this._dataSyncCache.Get(this._latestTableDataUploadTimeCacheKey, () =>
                {
                    return DateTime.Now.Date.Add(new TimeSpan(this._localPark.UploadTime.Hour, this._localPark.UploadTime.Minute, this._localPark.UploadTime.Second));
                });
            }
            private set
            {
                this._dataSyncCache.Set(this._latestTableDataUploadTimeCacheKey, value);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CenterDataSyncService(
            ICacheManager cacheManager,
            ISettingManager settingManager,
            ISyncParkAppService syncParkAppService,
            IDataSyncAppService dataSyncAppService,
            IBackgroundJobManager backgroundJobManager)
        {
            this.Logger = NullLogger.Instance;
            this._cacheManager = cacheManager;
            this._settingManager = settingManager;
            this._dataSyncAppService = dataSyncAppService;
            this._syncParkAppService = syncParkAppService;
            this._backgroundJobManager = backgroundJobManager;
            this._dataSyncCache = this._cacheManager.GetCache(DataSyncManager.DataSyncCacheKey);
        }

        /// <summary>
        /// 初始化中心数据同步服务
        /// </summary>
        public void InitCenterDataSyncService(int parkId, string syncSecretKey)
        {
            this._localParkId = parkId;
            this._syncSecretKey = syncSecretKey;
            if (this._localParkId != ThemeParkConsts.CenterParkId)
                this.Logger.Error($"配置错误，启动中心数据同步失败，Ps:中心的LocalParkId只能为{(int)ThemeParkConsts.CenterParkId}。。。");
            else
            {
                this.InitFileWatchForDownloadTableData();
                this._syncParks = this._syncParkAppService.GetAllList().Where(k => k.IsActive && !k.IsDeleted).ToList();//获取所有有效的需要进行数据同步公园
                this._localPark = this._syncParks.FirstOrDefault(k => k.ParkId == this._localParkId);
                if (this._localPark == null)
                    this.Logger.Error("配置错误，启动中心数据同步失败，获取不到LocalParkId对应的公园信息");
                else
                {
                    this._latestTableDataUploadTimeCacheKey = $"{this._localParkId:D5}_latestTableDataUploadTime";
                }
            }
        }

        /// <summary>
        /// 向公园上传表数据(修改/新增时间介于最后一次上传时间->结束时间的表数据)
        /// </summary>
        /// <param name="timeTo">结束时间</param>
        public async Task<Result> UploadTableDataToPark(DateTime timeTo)
        {
            var ss = LatestTableDataUploadTime;
            var result = await _dataSyncAppService.UploadTableUpdateFile(LatestTableDataUploadTime, timeTo);
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

                        this._backgroundJobManager.Enqueue<ApiDataSyncJob, ApiDataSyncJobArgs>(apiDataSyncJobArgs);
                    });
                }
                this.LatestTableDataUploadTime = timeTo;
            }

            return Result.Ok();
        }

        /// <summary>
        /// 向公园上传表数据((修改/新增时间介于开始时间->结束时间的表数据)
        /// </summary>
        /// <param name="timeFrom">开始时间</param>
        /// <param name="timeTo">结束时间</param>
        /// <returns></returns>
        public async Task<Result> UploadTableDataToPark(DateTime timeFrom, DateTime timeTo)
        {
            var result = await _dataSyncAppService.UploadTableUpdateFile(timeFrom, timeTo);
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

                        this._backgroundJobManager.Enqueue<ApiDataSyncJob, ApiDataSyncJobArgs>(apiDataSyncJobArgs);
                    });
                }
            }

            return Result.Ok();
        }

        /// <summary>
        /// 启动文件监视以下载公园同步上来的表数据
        /// </summary>
        private void InitFileWatchForDownloadTableData()
        {
            // 监控上传文件
            var fileWatcher = new FileSystemWatcher
            {
                Path = _settingManager.GetSettingValueForApplication(DataSyncSetting.UploadFilePath),
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

            this._backgroundJobManager.Enqueue<CenterTableDataDownloadJob, CenterTableDataDownloadArgs>(centerTableDataDownloadArgs);
        }
    }
}
