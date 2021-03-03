using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThemePark.Application.Agencies.Dto
{
    /// <summary>
    /// 查询旅行社充值记录
    /// </summary>
    public class GetAccountOpRecord
    {
        /// <summary>
        /// 查询开始时间
        /// </summary>
        public DateTime? SearchStarTime { get; set; }

        /// <summary>
        /// 查询结束时间
        /// </summary>
        public DateTime? SearchEnDateTime { get; set; }

        /// <summary>
        /// 代理商ID
        /// </summary>
        public int AgencyId { get; set; }
    }
}
