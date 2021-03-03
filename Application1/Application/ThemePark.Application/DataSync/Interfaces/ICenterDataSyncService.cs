using System;
using System.Threading.Tasks;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.DataSync.Interfaces
{
    /// <summary>
    /// 公园到中心数据同步接口
    /// </summary>
    public interface ICenterDataSyncService
    {
        /// <summary>
        /// 初始化中心数据同步服务
        /// </summary>
        /// <param name="parkId">公园Id</param>
        /// <param name="syncSecretKey">数据同步密钥</param>
        void InitCenterDataSyncService(int parkId, string syncSecretKey);

        /// <summary>
        /// 向公园上传表数据(修改/新增时间介于最后一次上传时间->结束时间的表数据)
        /// </summary>
        /// <param name="timeTo">结束时间</param>
        Task<Result> UploadTableDataToPark(DateTime timeTo);

        /// <summary>
        /// 向公园上传表数据((修改/新增时间介于开始时间->结束时间的表数据)
        /// </summary>
        /// <param name="timeFrom">开始时间</param>
        /// <param name="timeTo">结束时间</param>
        /// <returns></returns>
        Task<Result> UploadTableDataToPark(DateTime timeFrom, DateTime timeTo);

    }
}
