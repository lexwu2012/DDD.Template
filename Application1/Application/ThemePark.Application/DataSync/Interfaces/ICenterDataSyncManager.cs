using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.DataSync.Interfaces
{
    /// <summary>
    /// 中心数据同步管理接口
    /// </summary>
    public interface ICenterDataSyncManager
    {
        /// <summary>
        /// 向公园上传表数据(修改/新增时间介于最后一次上传时间->当前时间的表数据)
        /// </summary>
        /// <param name="currentTime">当前时间</param>
        Task<Result> UploadTableDataToPark(DateTime currentTime);

        /// <summary>
        /// 向公园上传表数据((修改/新增时间介于开始时间->结束时间的表数据)
        /// </summary>
        /// <param name="timeFrom">开始时间</param>
        /// <param name="timeTo">结束时间</param>
        /// <returns></returns>
        Task<Result> UploadTableDataToPark(DateTime timeFrom, DateTime timeTo);

        /// <summary>
        /// 向特定公园上传表数据(修改/新增时间介于开始时间->结束时间的表数据)
        /// </summary>
        /// <param name="targetParkIds"></param>
        /// <param name="timeFrom">开始时间</param>
        /// <param name="timeTo">结束时间</param>
        /// <returns></returns>
        Task<Result> UploadTableDataToTargetPark(List<int> targetParkIds, DateTime timeFrom, DateTime timeTo);

        /// <summary>
        /// 启动文件监视以下载公园同步上来的表数据
        /// </summary>
        void InitFileWatchForDownloadTableData();
    }
}
