using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ThemePark.Core.BasicData;

namespace ThemePark.ApplicationDto.BasicData
{
    /// <summary>
    /// 数据权限 完整DTO对象
    /// </summary>
    /// <seealso cref="Abp.Application.Services.Dto.FullAuditedEntityDto" />
    [AutoMapFrom(typeof(ParkArea))]
    public class ParkAreaDto : FullAuditedEntityDto
    {
        #region Properties

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 父节点
        /// </summary>
        //public virtual ParkArea Parent { get; set; }

        /// <summary>
        /// 父节点Id
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// 到父节点的路径
        /// </summary>
        public string ParentPath { get; set; }

        /// <summary>
        /// 只有叶子节点值不为空
        /// </summary>
        public int? ParkId { get; set; }

        #endregion Properties
    }
}