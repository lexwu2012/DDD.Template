using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System.Collections.Generic;
using ThemePark.Core.BasicData;

namespace ThemePark.ApplicationDto.BasicData
{
    /// <summary>
    /// 公园分区树形DTO对象
    /// </summary>
    /// <seealso cref="Abp.Application.Services.Dto.EntityDto"/>
    [AutoMapFrom(typeof(ParkArea))]
    public class ParkAreaTreeDto : EntityDto
    {
        #region Properties

        /// <summary>
        /// 孩子节点
        /// </summary>
        /// <value>The children.</value>
        public IList<ParkAreaTreeDto> Children { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 树结构层级，1代表root
        /// </summary>
        /// <value>The level.</value>
        public int Level { get; set; }

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