using Abp.BackgroundJobs;
using Abp.Dependency;
using ThemePark.Application.DataSync.Dto;
using ThemePark.Application.DataSync.Interfaces;

namespace ThemePark.Application.DataSync
{
    /// <summary>
    /// 公园汇总信息上传参数
    /// </summary>
    public class ParkSummarizingInfoUploadArgs : SumParkInfoDto
    {
        ///// <summary>
        ///// 构造函数
        ///// </summary>
        ///// <param name="sumParkInfoDto"></param>
        //public ParkSummarizingInfoUploadArgs(SumParkInfoDto sumParkInfoDto)
        //{
        //    ParkId = sumParkInfoDto.ParkId;
        //    InParkCount = sumParkInfoDto.InParkCount;
        //    SaleAmount = sumParkInfoDto.SaleAmount;//从交易记录里面取金额
        //}
    }

    /// <summary>
    /// 公园汇总信息上传Job
    /// </summary>
    public class ParkSummarizingInfoUploadJob : BackgroundJob<ParkSummarizingInfoUploadArgs>, ITransientDependency
    {
        private readonly IDataSyncAppService _dataSyncAppService;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ParkSummarizingInfoUploadJob(IDataSyncAppService dataSyncAppService)
        {
            this._dataSyncAppService = dataSyncAppService;
        }

        /// <summary>
        /// 汇总信息上传
        /// </summary>
        public override void Execute(ParkSummarizingInfoUploadArgs args)
        {
            var sumParkInfoDto = args as SumParkInfoDto;
            if (sumParkInfoDto != null)
            {
                this._dataSyncAppService.UploadSumInfo(sumParkInfoDto);
            }
        }
    }
}
