using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThemePark.Application.Agencies.Dto
{
    /// <summary>
    /// 获取代理商挂账余额
    /// </summary>
    public class GetAgencyBalanceInput
    {
        /// <summary>
        /// 代理商ID
        /// </summary>
        public int AgencyId { get; set; }
    }
}
