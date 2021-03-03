using Abp.BackgroundJobs;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Runtime.Caching;
using Abp.Threading;
using Castle.Core.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ThemePark.Application.DataSync.Dto;
using ThemePark.Application.DataSync.Interfaces;
using ThemePark.Application.Message;
using ThemePark.Common;
using ThemePark.Core.DataSync;
using ThemePark.Core.Settings;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.DataSync
{
    /// <summary>
    /// 数据同步管理
    /// </summary>
    public class DataSyncManager : IDataSyncManager
    {
        /// <summary>
        /// 数据同步CacheKey
        /// </summary>
        public static readonly string DataSyncCacheKey = "ParkSyncCache";

        private readonly IList<string> _exceptionAlarmPhones;

        private string _syncSecretKey { get; set; }//数据同步密钥

        #region Properties
        /// <summary>
        /// 日志
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// 数据同步缓存
        /// </summary>
        protected ICache DataSyncCache { get; private set; }

        /// <summary>
        /// 配置管理
        /// </summary>
        protected ISettingManager SettingManager { get; private set; }

        /// <summary>
        /// 数据同步服务方法
        /// </summary>
        protected IDataSyncAppService DataSyncAppService { get; private set; }

        /// <summary>
        /// 后台任务管理
        /// </summary>
        protected IBackgroundJobManager BackgroundJobManager { get; private set; }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public DataSyncManager(
            ICacheManager cacheManager,
            ISettingManager settingManager,
            IDataSyncAppService dataSyncAppService,
            IBackgroundJobManager backgroundJobManager)
        {
            this.SettingManager = settingManager;
            this.DataSyncAppService = dataSyncAppService;
            this.BackgroundJobManager = backgroundJobManager;
            this.Logger = NullLogger.Instance;
            this.DataSyncCache = cacheManager.GetCache(DataSyncManager.DataSyncCacheKey);

            this._syncSecretKey = this.SettingManager.GetSettingValueForApplication(DataSyncSetting.DataSyncSecretKey);
            this._exceptionAlarmPhones = System.Configuration.ConfigurationManager.AppSettings["AlarmPhone"]?.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// 向目标公园上传数据
        /// </summary>
        /// <param name="syncPark">目标公园信息</param>
        /// <param name="dataSyncInput">上传信息</param>
        public async Task<string> UploadDataToTargetParkAsync(SyncPark syncPark, DataSyncInput dataSyncInput)
        {
            var uri = new Uri(syncPark.SyncUrl);
            var requestUri = uri.AbsolutePath;
            var baseUri = syncPark.SyncUrl.Replace(requestUri, "");
            var content = this.GetSignedUploadData(syncPark.ParkId, dataSyncInput);

            return await HttpHelper.PostAsync(baseUri, requestUri, content);
        }

        /// <summary>
        /// 向目标公园上传数据
        /// </summary>
        /// <param name="targetParkId">目标公园Id</param>
        /// <param name="dataSyncInput">上传信息</param>
        public void UploadDataToTargetPark(int targetParkId, DataSyncInput dataSyncInput)
        {
            var apiDataSyncJobArgs = new ApiDataSyncJobArgs
            {
                TargetParkId = targetParkId,
                Sign = dataSyncInput.Sign,
                TaskId = dataSyncInput.TaskId,
                CreationTime = dataSyncInput.CreationTime,
                SyncType = dataSyncInput.SyncType,
                FromPark = dataSyncInput.FromPark,
                SyncData = dataSyncInput.SyncData,
            };

            this.BackgroundJobManager.Enqueue<ApiDataSyncJob, ApiDataSyncJobArgs>(apiDataSyncJobArgs);
        }

        /// <summary>
        /// 获取经过签名处理的上传数据
        /// </summary>
        /// <param name="toParkId">目标公园Id</param>
        /// <param name="dataSyncInput">同步数据</param>
        /// <returns></returns>
        public string GetSignedUploadData(int toParkId, DataSyncInput dataSyncInput)
        {
            var signature = this.GetSignature(dataSyncInput.SyncType, dataSyncInput.SyncData);
            dataSyncInput.Sign = signature;

            return JsonConvert.SerializeObject(dataSyncInput);
        }

        /// <summary>
        /// 获取上传数据签名
        /// </summary>
        /// <param name="dataSyncType">上传数据类型</param>
        /// <param name="syncData">上传数据</param>
        /// <returns></returns>
        protected string GetSignature(DataSyncType dataSyncType, string syncData)
        {
            var rawSignature = $"{dataSyncType.ToString()}{ syncData }{this._syncSecretKey}";
            var md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.Unicode.GetBytes(rawSignature);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x");
            }

            return byte2String;
        }

        /// <summary>
        /// 记录数据同步日志
        /// </summary>
        protected void WriteDataSyncLog(int fromParkId, int toParkId, DataSyncType syncType, string requestUrl, string syncInfo, ResultCode resultCode, Exception exception = null, string remark = "")
        {
            var taskId = Guid.NewGuid();
            var errorMessage = exception?.GetAllExceptionMessage().JoinAsString(Environment.NewLine);

            this.DataSyncAppService.AddSyncLog
                (fromParkId, toParkId, syncType, requestUrl, syncInfo, (int)resultCode, taskId, $"{remark} {errorMessage}");
        }

        /// <summary>
        /// 发送异常报警信息
        /// </summary>
        protected void SendAlarmMessage(int localParkId, Exception exception)
        {
            if (this._exceptionAlarmPhones.Any())
            {
                var alarmMessage = $"公园编号：{localParkId}\r\n{exception?.Message}";
                using (var smsAppService = IocManager.Instance.ResolveAsDisposable<ISmsAppService>())
                {
                    smsAppService.Object.SendMessage(this._exceptionAlarmPhones, alarmMessage);
                }
            }
        }
    }
}
