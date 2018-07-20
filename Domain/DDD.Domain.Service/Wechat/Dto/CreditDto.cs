using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Domain.Service.Wechat.Dto
{
    public class CreditDto
    {
        public CreditDto()
        {
            InstalmentList = new List<InstalmentDto>();
        }

        /// <summary>
        /// 期次列表
        /// </summary>
        public List<InstalmentDto> InstalmentList { get; set; }
      
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
        /// 产品类型
        /// </summary>
        public string ProductType { get; set; }

        /// <summary>
        /// 申请日期
        /// </summary>
        public string AppDate { get; set; }

        /// <summary>
        /// 现行日期
        /// </summary>
        public string LoanDate { get; set; }

        /// <summary>
        /// 分期期数
        /// </summary>
        public int? PaymentNum { get; set; }
    }
}
