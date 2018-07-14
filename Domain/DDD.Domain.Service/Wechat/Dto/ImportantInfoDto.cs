using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Domain.Service.Wechat.Dto
{
    public class ImportantInfoDto
    {
        internal Func<string> GetEndDate;
        internal Func<bool> GetIsOverDuce;
        internal Func<decimal> GetMaxRepayAmount;
        internal Func<decimal> GetCycleTotalAmount;
        internal Func<decimal> GetRepayAmount;
        internal Func<decimal> GetInstalmentAmount;
        internal Func<decimal> GetTotalInstalmentAmount;

        /// <summary>
        /// 最后还款日期
        /// </summary>
        public string EndDate => GetEndDate();

        /// <summary>
        /// 是否已逾期
        /// </summary>
        public bool IsOverDuce => GetIsOverDuce();

        /// <summary>
        /// 总欠款
        /// </summary>
        public decimal MaxRepayAmount => GetMaxRepayAmount();

        /// <summary>
        /// 零花钱总欠款
        /// </summary>
        public decimal CycleTotalAmount => GetCycleTotalAmount();

        /// <summary>
        /// 非零花钱总欠款
        /// </summary>
        public decimal OthersTotalAmount => MaxRepayAmount - CycleTotalAmount;

        /// <summary>
        /// 当期欠款
        /// </summary>
        public decimal RepayAmount => GetRepayAmount();

        /// <summary>
        /// 还款日期
        /// </summary>
        public string RepayDate => EndDate;

        /// <summary>
        /// 当期期款
        /// </summary>
        public decimal InstalmentAmount => GetInstalmentAmount();

        /// <summary>
        /// 总期款
        /// </summary>
        public decimal TotalInstalmentAmount => GetTotalInstalmentAmount();
    }
}
