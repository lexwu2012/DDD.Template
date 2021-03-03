using Abp.BackgroundJobs;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Threading;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Linq;
using ThemePark.Application.DataSync.Dto;
using ThemePark.Application.DataSync.Interfaces;
using ThemePark.Common;
using ThemePark.Core.DataSync;
using ThemePark.Core.Settings;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.DataSync
{
    /// <summary>
    /// Api数据同步Job
    /// </summary>
    public class ApiDataSyncJob : BackgroundJob<ApiDataSyncJobArgs>, ITransientDependency
    {
        private string _syncSecretKey { get; set; }//数据同步密钥
        private readonly ISettingManager _settingManager;
        private readonly IDataSyncManager _dataSyncManager;
        private readonly IDataSyncAppService _dataSyncAppService;
        private readonly IRepository<SyncPark> _syncParkRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ApiDataSyncJob(ISettingManager settingManager
            , IDataSyncManager dataSyncManager
            , IDataSyncAppService dataSyncAppService
            , IRepository<SyncPark> syncParkRepository)
        {
            this._syncParkRepository = syncParkRepository;
            this._dataSyncAppService = dataSyncAppService;
            this._settingManager = settingManager;
            this._dataSyncManager = dataSyncManager;
            this._syncSecretKey = this._settingManager.GetSettingValueForApplication(DataSyncSetting.DataSyncSecretKey);
        }

        /// <summary>
        /// 通过Api调用的方式发起数据同步
        /// </summary>
        public override void Execute(ApiDataSyncJobArgs args)
        {
            var result = Result.FromCode(ResultCode.Fail);
            var dataSyncInput = args as DataSyncInput;
            var syncPark = this._syncParkRepository.FirstOrDefault(k => k.ParkId == args.TargetParkId);
            if (syncPark == null || dataSyncInput == null)
                result.Message = "参数错误，无对应公园信息或同步数据";
            else
            {
                // syncPark.SyncUrl = "http://10.98.56.183:8005/Api/DataSync/Post";//本地测试
                var uri = new Uri(syncPark.SyncUrl);
                var requestUri = uri.AbsolutePath;
                var baseUri = syncPark.SyncUrl.Replace(requestUri, "");
                var content = this._dataSyncManager.GetSignedUploadData(syncPark.ParkId, dataSyncInput);
                var httpResponse = AsyncHelper.RunSync(() => HttpHelper.PostRequestAsync(baseUri, requestUri, content));
                if (!httpResponse.IsSuccessStatusCode)//发送数据同步请求不成功
                    result.Message = $"向公园【{syncPark.ParkId:D5}】发起{dataSyncInput.SyncType.ToString()}数据同步失败\r\n异常信息：\r\n{httpResponse.Content}";
                else//数据同步业务逻辑处理不成功
                {
                    var responseContent = AsyncHelper.RunSync(() => httpResponse.Content.ReadAsStringAsync());
                    result = JsonConvert.DeserializeObject<Result>(responseContent);
                    if (result != null && !result.Success)
                        result.Message = $"向公园【{syncPark.ParkId:D5}】发起{dataSyncInput.SyncType.ToString()}数据同步失败\r\n失败信息：\r\n{result.Message}";
                }

                //记录数据同步日志
                this._dataSyncAppService
                    .AddSyncLog(int.Parse(dataSyncInput.FromPark), syncPark.ParkId
                    , dataSyncInput.SyncType, syncPark.SyncUrl, content, (int)result.Code, dataSyncInput.TaskId, result.Message);
            }

            if (!result.IsOk())//数据同步不成功时抛出异常，以便触发重试功能
            {
                base.Logger.Error(result.Message);
                throw new Exception(result.Message);
            }
        }
    }

    /// <summary>
    /// Api数据同步参数
    /// </summary>
    public class ApiDataSyncJobArgs : DataSyncInput
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ApiDataSyncJobArgs()
        {
            base.TaskId = Guid.NewGuid();
            base.CreationTime = DateTime.Now;
            base.FromPark = ConfigurationManager.AppSettings[AppConfigSetting.LocalParkId];
        }

        /// <summary>
        /// 目标公园Id
        /// </summary>
        public int TargetParkId { get; set; }
    }
}
