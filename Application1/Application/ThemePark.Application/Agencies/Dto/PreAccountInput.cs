using Abp.AutoMapper;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ThemePark.Core.Agencies;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.Agencies.Dto
{
    /// <summary>
    /// 
    /// </summary>
    [AutoMap(typeof(Agency))]
    public class PreAccountInput
    {
        /// <summary>
        /// 代理商账户Id
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// 代理商名称
        /// </summary>
        [DisplayName("旅游网")]
        public string AgencyName { get; set; }

        /// <summary>
        /// 账户/单位名称
        /// </summary>  
        [DisplayName("单位名称")]
        [MapFrom(nameof(Agency.Account), nameof(Account.AccountName))]
        public string AccountName { get; set; }

        /// <summary>
        /// 预付款
        /// </summary> 
        [DisplayName("预付款")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "必须为数字，且不能为空")]
        [MapFrom(nameof(Agency.Account), nameof(Account.Balance))]
        public decimal Balance { get; set; }

        /// <summary>
        /// 预警金额
        /// </summary>
        [DisplayName("预警金额")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "必须为数字，且不能为空"), Range(typeof(decimal), "-1000000000.00", "1000000000.00", ErrorMessage = "数值大小必须介于-1000000000-1000000000之间")]
        [MapFrom(nameof(Agency.Account), nameof(Account.AlarmBalance))]
        public decimal AlarmBalance { get; set; }

        /// <summary>
        /// 最低金额（低于该数值会下单失败）
        /// </summary>
        [DisplayName("最低金额")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "必须为数字，且不能为空"), Range(typeof(decimal), "-1000000000.00", "1000000000.00", ErrorMessage = "数值大小必须介于-1000000000-1000000000之间")]
        [MapFrom(nameof(Agency.Account), nameof(Account.LeastBalance))]
        public decimal LeastBalance { get; set; }

        /// <summary>
        /// 预存金额小于等于预警金额时需要通知的方特人员手机号，多个手机号要用逗号隔开(譬如：15111111111,15222222222)
        /// </summary>
        [DisplayName("方特预警接收手机号")]
        [MaxLength(500, ErrorMessage = "不能超过500个字符")]
        [MapFrom(nameof(Agency.Account), nameof(Account.FtPeoplePhone))]
        public string FtPeoplePhone { get; set; }

        /// <summary>
        /// 预存金额小于等于预警金额时需要通知的旅游网预付款负责人手机号，多个手机号要用逗号隔开(譬如：15111111111,15222222222)
        /// </summary>
        [DisplayName("旅行社预警接收手机号")]
        [MaxLength(500, ErrorMessage = "不能超过500个字符")]
        [MapFrom(nameof(Agency.Account), nameof(Account.AgencyPeoplePhone))]
        public string AgencyPeoplePhone { get; set; }

        /// <summary>
        /// 备注
        /// </summary>  
        [DisplayName("备注")]
        public string Remark { get; set; }

    }
}
