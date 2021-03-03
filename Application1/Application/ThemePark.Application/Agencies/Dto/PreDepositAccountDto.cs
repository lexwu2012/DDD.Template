using System.Collections.Generic;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.Agencies.Dto
{
    /// <summary>
    /// 挂账页面模型
    /// </summary>
    public class PreDepositAccountDto
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PreDepositAccountDto()
        {
            AccountOpDtoList = new List<AccountOpDto>();
        }

        /// <summary>
        /// 代理商下拉列表
        /// </summary>
        public DropdownDto AgencyList { get; set; }

        /// <summary>
        /// 默认代理商的账户操作历史
        /// </summary>
        public IList<AccountOpDto> AccountOpDtoList { get; set; }

        /// <summary>
        /// 默认代理商的账户余额
        /// </summary>
        public decimal AgencyAccountBalance { get; set; }
    }
}
