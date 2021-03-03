using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.AutoMapper;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.Agencies.Dto
{
    /// <summary>
    /// 
    /// </summary>
    [AutoMapFrom(typeof(ParkArea))]
    public class AccountParkAreaDto
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 到父节点的路径，<see cref="Separator"/>间隔，<see cref="TopPath"/>代表root
        /// </summary>
        public string ParentPath { get; set; }

        /// <summary>
        /// 只有叶子节点值不为空
        /// </summary>
        public int? ParkId { get; set; }
    }
}
