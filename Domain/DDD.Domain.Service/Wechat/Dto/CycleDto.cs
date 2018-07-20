using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Domain.Service.Wechat.Dto
{
    public class CycleDto
    {
        /// <summary>
        /// 合同ID
        /// </summary>
        public long ContractId { get; set; }

        /// <summary>
        /// 合同号
        /// </summary>
        public long ContractNo { get; set; }

        /// <summary>
        /// 合同类型
        /// </summary>
        public string CreditType { get; set; }

        /// <summary>
        /// 是否有处理中的代扣记录
        /// </summary>
        public bool IsWithholding { get; set; }

        /// <summary>
        /// 是否已逾期
        /// </summary>
        public bool IsOverduce { get; set; }

        /// <summary>
        /// 当前欠款
        /// </summary>
        public decimal RepayAmount { get; set; }

        /// <summary>
        /// 还款日期
        /// </summary>
        public string RepayDate { get; set; }

        /// <summary>
        /// 现行日期
        /// </summary>
        public string LoanDate { get; set; }

        /// <summary>
        /// 贷款总费用
        /// </summary>
        public decimal TotalAmount { get; set; }
    }
}
