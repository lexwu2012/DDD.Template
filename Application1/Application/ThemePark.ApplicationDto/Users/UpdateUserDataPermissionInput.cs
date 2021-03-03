using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ThemePark.Core.Users;

namespace ThemePark.ApplicationDto.Users
{
    /// <summary>
    /// 修改用户数据权限
    /// </summary>
    /// <seealso cref="Abp.Application.Services.Dto.EntityDto{System.Int64}"/>
    [AutoMapTo(typeof(SysUser))]
    public class UpdateUserDataPermissionInput : EntityDto<long>
    {
        #region Properties

        /// <summary>
        /// 所属公园分区
        /// </summary>
        public int? ParkAreaId { get; set; }

        #endregion Properties
    }
}