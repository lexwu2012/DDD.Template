using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThemePark.Application.DataSync.Dto;
using ThemePark.Core.DataSync;

namespace ThemePark.Application.DataSync.Interfaces
{
    /// <summary>
    /// 数据同步管理接口
    /// </summary>
    public interface IDataSyncManager
    {
        /// <summary>
        /// 向目标公园上传数据
        /// </summary>
        /// <param name="syncPark">目标公园信息</param>
        /// <param name="dataSyncInput">上传信息</param>
        Task<string> UploadDataToTargetParkAsync(SyncPark syncPark, DataSyncInput dataSyncInput);

        /// <summary>
        /// 向目标公园上传数据
        /// </summary>
        /// <param name="targetParkId">目标公园Id</param>
        /// <param name="dataSyncInput">上传信息</param>
        void UploadDataToTargetPark(int targetParkId, DataSyncInput dataSyncInput);

        /// <summary>
        /// 获取经过签名处理的上传数据
        /// </summary>
        /// <param name="toParkId">目标公园Id</param>
        /// <param name="dataSyncInput">同步数据</param>
        /// <returns></returns>
        string GetSignedUploadData(int toParkId, DataSyncInput dataSyncInput);
    }
}
