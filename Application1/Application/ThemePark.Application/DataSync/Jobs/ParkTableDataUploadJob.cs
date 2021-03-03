using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Threading;
using System;
using ThemePark.Application.DataSync.Interfaces;

namespace ThemePark.Application.DataSync
{
    /// <summary>
    /// 公园表数据上传参数
    /// </summary>
    public class ParkTableDataUploadArgs
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
    /// 公园表数据上传Job
    /// </summary>
    public class ParkTableDataUploadJob : BackgroundJob<ParkTableDataUploadArgs>, ITransientDependency
    {
        private readonly IDataSyncAppService _dataSyncAppService;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ParkTableDataUploadJob(IDataSyncAppService dataSyncAppService)
        {
            this._dataSyncAppService = dataSyncAppService;
        }

        /// <summary>
        /// 表数据同步
        /// </summary>
        public override void Execute(ParkTableDataUploadArgs args)
        {
            var result = AsyncHelper.RunSync(() => this._dataSyncAppService.UploadTableUpdateFile(args.TimeFrom, args.TimeTo));
           if (!result.Success) { throw new Exception(result.Message); }
        }
    }
}
