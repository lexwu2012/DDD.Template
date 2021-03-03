using Abp.BackgroundJobs;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Runtime.Caching;
using Abp.Timing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThemePark.Application.DataSync.Dto;
using ThemePark.Application.DataSync.Interfaces;
using ThemePark.Core;
using ThemePark.Core.DataSync;
using ThemePark.Core.Settings;
using ThemePark.EntityFramework;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.DataSync
{
    /// <summary>
    /// 公园数据同步管理
    /// </summary>
    public class ParkDataSyncManager : DataSyncManager, IParkDataSyncManager
    {
        #region Fields
        private readonly int _localParkId;//本地(服务部署)公园Id
        private readonly SyncPark _localPark;//本地(服务部署)公园信息
        private readonly IList<SyncPark> _syncParks;//需要进行数据同步的公园

        private int _summarizingInfoUploadStartHour = 0;//汇总信息上传开始钟点
        private int _summarizingInfoUploadEndHour = 24;//汇总信息上传结束钟点
        private string _latestTableDataUploadTimeCacheKey;//最后一次表数据上传时间缓存Key
        private string _latestTableDataDownloadTimeCacheKey;//最后一次表数据下载时间缓存Key

        private readonly ISyncParkAppService _syncParkAppService;
        #endregion

        #region Properties
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
        /// 最后一次表数据下载时间
        /// </summary>
        public DateTime LatestTableDataDownloadTime
        {
            get
            {
                return base.DataSyncCache.Get(this._latestTableDataDownloadTimeCacheKey, () =>
                {
                    return DateTime.Now.Date.Add(new TimeSpan(this._localPark.DownloadTime.Hour, this._localPark.DownloadTime.Minute, this._localPark.DownloadTime.Second));
                });
            }
            private set
            {
                base.DataSyncCache.Set(this._latestTableDataDownloadTimeCacheKey, value);
            }
        }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public ParkDataSyncManager(
            ICacheManager cacheManager,
            ISettingManager settingManager,
            IDataSyncAppService dataSyncAppService,
            ISyncParkAppService syncParkAppService,
            IBackgroundJobManager backgroundJobManager)
            : base(cacheManager, settingManager, dataSyncAppService, backgroundJobManager)
        {
            this._syncParkAppService = syncParkAppService;

            this._localParkId = base.SettingManager.GetSettingValueForApplication<int>(DataSyncSetting.LocalParkId);
            this._syncParks = this._syncParkAppService.GetAllList().Where(k => k.IsActive && !k.IsDeleted).ToList();//获取所有有效的需要进行数据同步的公园
            this._localPark = this._syncParks.FirstOrDefault(k => k.ParkId == this._localParkId);

            var currentTime = Clock.Now;
            this._latestTableDataUploadTimeCacheKey = $"{this._localParkId:D5}_latestTableDataUploadTime";
            this._latestTableDataDownloadTimeCacheKey = $"{this._localParkId:D5}_latestTableDataDownLoadTime";
            this._summarizingInfoUploadStartHour = base.SettingManager.GetSettingValueForApplication<int>(DataSyncSetting.UploadSumInfoStartHour);
            this._summarizingInfoUploadEndHour = base.SettingManager.GetSettingValueForApplication<int>(DataSyncSetting.UploadSumInfoEndHour);
        }


        /// <summary>
        /// 向中心上传当前汇总信息
        /// </summary>
        public async Task<Result> UploadSummarizingInfoToCenter()
        {
            var currentTime = Clock.Now;
            if (currentTime.Hour >= this._summarizingInfoUploadStartHour && currentTime.Hour <= this._summarizingInfoUploadEndHour)
            {
                var dateFrom = currentTime.Date;
                var dateTo = dateFrom;
                //dateFrom = DateTime.Parse("2017-10-12");
                //dateTo = DateTime.Parse("2017-10-12");
                var daySalesAmount = await this.GetSalesAmountByDate(dateFrom, dateTo);//当天销售总额
                var dayInParkTimes = await this.GetInParkTimesByDate(dateFrom, dateTo);//当天入园人次
                var sumParkInfoDto = new SumParkInfoDto
                {
                    ParkId = this._localParkId,
                    InParkCount = dayInParkTimes,
                    SaleAmount = daySalesAmount,
                    SummarizeDate = currentTime.Date
                };
                var apiDataSyncJobArgs = new ApiDataSyncJobArgs
                {
                    TargetParkId = ThemeParkConsts.CenterParkId,
                    SyncData = JsonConvert.SerializeObject(sumParkInfoDto),
                    SyncType = DataSyncType.UploadSumInfo
                };

                await Task.Run(() => base.BackgroundJobManager.Enqueue<ApiDataSyncJob, ApiDataSyncJobArgs>(apiDataSyncJobArgs));
            }

            return Result.Ok();
        }

        /// <summary>
        /// 向中心上传表数据(修改/新增时间介于最后一次上传时间->当前时间的表数据)
        /// </summary>
        /// <param name="currentTime">结束时间</param>
        public async Task<Result> UploadTableDataToCenter(DateTime currentTime)
        {
            var result = await base.DataSyncAppService.UploadTableUpdateFile(this.LatestTableDataUploadTime, currentTime);
            if (!result.Success)
                throw new Exception(result.Message);
            else
            {
                this.LatestTableDataUploadTime = currentTime;
                return Result.Ok();
            }
        }

        /// <summary>
        /// 向中心上传表数据(修改/新增时间介于开始时间->结束时间的表数据)
        /// </summary>
        /// <param name="timeFrom">开始时间</param>
        /// <param name="timeTo">结束时间</param>
        /// <returns></returns>
        public async Task<Result> UploadTableDataToCenter(DateTime timeFrom, DateTime timeTo)
        {
            var result = await base.DataSyncAppService.UploadTableUpdateFile(timeFrom, timeTo);
            if (!result.Success)
                throw new Exception(result.Message);
            else
            {
                this.LatestTableDataUploadTime = timeTo;
                return Result.Ok();
            }
        }

        /// <summary>
        /// 获取特定日期之间的销售额
        /// </summary>
        /// <param name="dateFrom">开始日期</param>
        /// <param name="dateTo">结束日期</param>
        /// <returns></returns>
        public async Task<decimal> GetSalesAmountByDate(DateTime dateFrom, DateTime dateTo)
        {
            var salesAmount = 0M;
            var sqlparameters = new SqlParameter[] {
                new SqlParameter { ParameterName = "ParkId", Value = this._localParkId },
                new SqlParameter { ParameterName = "CreationTimeStart", Value = dateFrom },
                new SqlParameter { ParameterName = "CreationTimeEnd", Value = dateTo }
            };

            using (var dbcontext = IocManager.Instance.Resolve<ThemeParkDbContext>())
            {
                var dataSet = await Task.Run(() => dbcontext.ExecStoreQuery("pr_SalesSummary", sqlparameters));
                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    var dataRow = dataSet.Tables[0].AsEnumerable()
                        .FirstOrDefault(k => k.Field<string>("TypeName") == "交易总计");
                    if (dataRow != null)
                    {
                        decimal.TryParse(dataRow["Amount"]?.ToString(), out salesAmount);
                    }
                }

                return salesAmount;
            }
        }

        /// <summary>
        /// 获取特定日期之前的入园人数
        /// </summary>
        /// <param name="dateFrom">开始日期</param>
        /// <param name="dateTo">结束日期</param>
        /// <returns></returns>
        public async Task<int> GetInParkTimesByDate(DateTime dateFrom, DateTime dateTo)
        {
            var inParkTimes = 0;
            var sqlparameters = new SqlParameter[] {
                new SqlParameter { ParameterName = "ParkId", Value = this._localParkId},
                new SqlParameter { ParameterName = "CreationTimeStart", Value = dateFrom },
                new SqlParameter { ParameterName = "CreationTimeEnd", Value = dateTo }
            };

            using (var dbcontext = IocManager.Instance.Resolve<ThemeParkDbContext>())
            {
                var dataSet = await Task.Run(() => dbcontext.ExecStoreQuery("audit_Inpark", sqlparameters));
                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    var dataRow = dataSet.Tables[0].AsEnumerable()
                        .FirstOrDefault(k => k.Field<string>("CreationTime") == "总计");
                    if (dataRow != null)
                    {
                        int.TryParse(dataRow["Qty"]?.ToString(), out inParkTimes);
                    }
                }

                return inParkTimes;
            }
        }
    }
}
