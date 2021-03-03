using System.Collections.Generic;
using Abp.AutoMapper;
using ThemePark.Core.BasicData;

namespace ThemePark.ApplicationDto.BasicData
{
    /// <summary>
    /// 更新公园分区 树形结构
    /// </summary>
    public class UpdateParkAreaTreeInput : List<UpdateParkAreaTreeNode>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateParkAreaTreeInput"/> class.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        public UpdateParkAreaTreeInput(IEnumerable<UpdateParkAreaTreeNode> nodes)
        {
            this.AddRange(nodes);
        }
    }

    /// <summary>
    /// 树节点
    /// </summary>
    [AutoMapTo(typeof(ParkArea))]
    public class UpdateParkAreaTreeNode
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }

        /// <summary>
        /// 父节点Id
        /// </summary>
        public int? ParentId { get; set; }
    }
}
