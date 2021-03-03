using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using ThemePark.Application.DataSync.Dto;
using ThemePark.Core.DataSync;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.DataSync.Interfaces
{
    /// <summary>
    /// 公园数据同步接口，由中心服务程序调用
    /// </summary>
    public interface IDataSyncAppService : IApplicationService
    {
        /// <summary>
        /// 上传数据更新文件
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        Task<Result> UploadTableUpdateFile(DateTime startTime, DateTime endTime);

        /// <summary>
        /// 下载数据更新文件
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        Result DownloadTableUpdateFile(DateTime startTime, DateTime endTime);

        /// <summary>
        /// 下载中心的数据更新文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        Result DownloadTableUpdateFile(string fileName);

        /// <summary>
        /// 更新数据表
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Result UpdateTables(string fileName);

        /// <summary>
        /// 增加数据同步日志
        /// </summary>
        /// <param name="fromParkId">From park identifier.</param>
        /// <param name="toParkId">To park identifier.</param>
        /// <param name="syncType">Type of the synchronize.</param>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="syncInfo">The synchronize information.</param>
        /// <param name="result">The result.</param>
        /// <param name="taskId">The task identifier.</param>
        /// <param name="remark">The remark.</param>
        /// <returns>System.Threading.Tasks.Task&lt;ThemePark.Infrastructure.Application.Result&gt;.</returns>
        Task<Result> AddSyncLog(int fromParkId, int toParkId, DataSyncType syncType, string requestUrl, string syncInfo,
            int result, Guid taskId, string remark = "");

        /// <summary>
        /// 地方公园上传了当天的汇总信息，中心在这里保存
        /// </summary>
        /// <param name="sumInfoDto"></param>
        /// <returns></returns>
        Task<Result> UploadSumInfo(SumParkInfoDto sumInfoDto);

        /// <summary>
        /// 获取任务完成状态
        /// </summary>
        /// <param name="taskId">The task identifier.</param>
        /// <returns>Task&lt;Result&gt;.</returns>
        Task<Result> GetTaskState(Guid taskId);
    }
}
