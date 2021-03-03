using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.AutoMapper;
using ThemePark.Core.Agencies;

namespace ThemePark.Application.DataSync.Dto
{
    /// <summary>
    /// 代理商账户数据同步
    /// </summary>
    [AutoMap(typeof(Account))]
    public class AccountDataSyncDto
    {

        /// <summary>
        /// id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 账户/单位名称
        /// </summary>    
        public string AccountName { get; set; }
    }
}
