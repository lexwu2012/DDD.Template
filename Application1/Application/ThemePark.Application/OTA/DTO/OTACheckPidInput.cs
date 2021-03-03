using System;

namespace ThemePark.Application.OTA.DTO
{
    /// <summary>
    /// 检查身份证入参
    /// </summary>
    public class CheckPidInput: ValidateParams
    {

        /// <summary>
        /// 身份证列表中间以“,”隔开如：“123456789012345678,123456789012345679”
        /// </summary>
        public string idnums { get; set; }

        /// <summary>
        /// 代理商ID
        /// </summary>
        public int AgencyId { get; set; }

        /// <summary>
        /// 有效开始日期
        /// </summary>
        public DateTime ValidStartDate { get; set; }

    }
}
