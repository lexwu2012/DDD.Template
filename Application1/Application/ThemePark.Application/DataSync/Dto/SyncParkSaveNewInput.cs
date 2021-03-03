using Abp.AutoMapper;
using System;
using System.ComponentModel.DataAnnotations;
using ThemePark.Core.DataSync;

namespace ThemePark.Application.DataSync.Dto
{
    /// <summary>
    /// 公园同步记录保存输入
    /// </summary>
    [AutoMap(typeof(SyncPark))]
    public class SyncParkSaveNewInput
    {
        /// <summary>
        /// 公园Id
        /// </summary>
        public int ParkId { get; set; }

        /// <summary>
        /// 同步网址
        /// </summary>
        public string SyncUrl { get; set; }

        /// <summary>
        /// 下载时间
        /// </summary>
        [DataType(DataType.Time)]
        public DateTime DownloadTime { get; set; }

        /// <summary>
        /// 上传时间
        /// </summary>
        [DataType(DataType.Time)]
        public DateTime UploadTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive { get; set; }
    }
}
