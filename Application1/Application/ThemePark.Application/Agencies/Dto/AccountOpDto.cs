using System;
using System.Collections.Generic;
using Abp.AutoMapper;
using Abp.Dependency;
using Nito.AsyncEx;
using ThemePark.Application.Users;
using ThemePark.Common;
using ThemePark.Core.Agencies;
using ThemePark.Core.TradeInfos;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.Agencies.Dto
{
    /// <summary>
    /// 账户操作记录
    /// </summary>
    [AutoMapFrom(typeof(AccountOp))]
    public class AccountOpDto 
    {

        /// <summary>
        /// 操作类型 充值 消费
        /// </summary>    
        public OpType OpType { get; set; }

        /// <summary>
        /// 操作类型名称
        /// </summary>
        public string OpTypeName => OpType.DisplayName();

        /// <summary>
        /// 备注
        /// </summary>    
        public string Remark { get; set; }

        /// <summary>
        /// 交易号
        /// </summary>
        public string TradeInfoId { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        [MapFrom(nameof(AccountOp.TradeInfo),nameof(TradeInfo.PayModeId))]
        public PayType PayModeId { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Cash { get; set; }

        /// <summary>
        /// 支付方式名称(挂账充值没有多种支付)
        /// </summary>
        public string PayModeName => PayModeId.DisplayName();

        /// <summary>
        /// 交易时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 操作用户
        /// </summary>
        public long? CreatorUserId { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorName
        {
            get
            {
                if (CreatorUserId.HasValue)
                {
                    return AsyncContext.Run(() => IocManager.Instance.Resolve<IUserAppService>().GetUserNameByIdAsync(CreatorUserId.Value));                    
                }

                return String.Empty;
            }
        }

    }
}
