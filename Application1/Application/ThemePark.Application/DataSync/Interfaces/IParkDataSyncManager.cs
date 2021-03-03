using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.DataSync.Interfaces
{
    /// <summary>
    /// 公园数据同步管理接口
    /// </summary>
    public interface IParkDataSyncManager
    {
        /// <summary>
        /// 向中心上传当前汇总信息
        /// </summary>
        Task<Result> UploadSummarizingInfoToCenter();

        /// <summary>
        /// 向中心上传表数据(修改/新增时间介于最后一次上传时间->结束时间的表数据)
        /// </summary>
        /// <param name="currentTime">结束时间</param>
        Task<Result> UploadTableDataToCenter(DateTime currentTime);

        /// <summary>
        /// 向中心上传表数据(修改/新增时间介于开始时间->结束时间的表数据)
        /// </summary>
        /// <param name="timeFrom">开始时间</param>
        /// <param name="timeTo">结束时间</param>
        /// <returns></returns>
        Task<Result> UploadTableDataToCenter(DateTime timeFrom, DateTime timeTo);

        /// <summary>
        /// 获取特定日期之间的销售额
        /// </summary>
        /// <param name="dateFrom">开始日期</param>
        /// <param name="dateTo">结束日期</param>
        /// <returns></returns>
        Task<decimal> GetSalesAmountByDate(DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// 获取特定日期之前的入园人数
        /// </summary>
        /// <param name="dateFrom">开始日期</param>
        /// <param name="dateTo">结束日期</param>
        /// <returns></returns>
        Task<int> GetInParkTimesByDate(DateTime dateFrom, DateTime dateTo);
    }
}
