using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Domain.Service.Wechat.Dto
{
    public class CycleInfoDto
    {
        public CycleInfoDto()
        {
            ContractList = new List<CycleDto>();
        }

        public List<CycleDto> ContractList { get; set; }

        /// <summary>
        /// 产品类型：c-零花钱，o-非零花钱
        /// </summary>
        public string ProductType { get; set; } = "c";

        /// <summary>
        /// 是否有处理中的代扣记录
        /// </summary>
        public bool IsWithholding { get; set; }

        /// <summary>
        /// 是否已逾期
        /// </summary>
        public bool IsOverduce => ContractList.Any(a => a.IsOverduce);
       
        /// <summary>
        /// 所有合同当前总欠款
        /// </summary>
        public decimal RepayAmount => Math.Round(ContractList.Sum(a => a.RepayAmount), 2, MidpointRounding.AwayFromZero);
        
        /// <summary>
        /// 所有合同当前总欠款
        /// </summary>
        public decimal MaxRepayAmount => RepayAmount;

        /// <summary>
        /// 所有合同贷款总费用
        /// </summary>
        public decimal TotalAmount => Math.Round(ContractList.Sum(a => a.TotalAmount), 2, MidpointRounding.AwayFromZero);
       
    }
}
