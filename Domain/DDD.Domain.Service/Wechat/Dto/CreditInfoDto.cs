using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Domain.Service.Wechat.Dto
{
    public class CreditInfoDto
    {
        public CreditInfoDto()
        {
            CreditDtoList = new List<CreditDto>();
        }

        public List<CreditDto> CreditDtoList { get; set; }

        /// <summary>
        /// 是否有处理中的代扣记录
        /// </summary>
        public bool IsWithholding { get; set; }

        /// <summary>
        /// 总欠款
        /// </summary>
        public decimal MaxRepayAmount => CreditDtoList.Sum(a => a.InstalmentList.Sum(b => b.RepayAmount));

        /// <summary>
        /// 当期欠款
        /// </summary>
        public decimal RepayAmount => Math.Round(CreditDtoList.Sum(a => a.InstalmentList.Sum(b => b.IsCurrent ? b.RepayAmount : 0)), 2, MidpointRounding.AwayFromZero);
        
        /// <summary>
        /// 当期期款
        /// </summary>
        public decimal InstalmentAmount => Math.Round(CreditDtoList.Sum(a => a.InstalmentList.Sum(b => b.IsCurrent ? b.instalmentAmount : 0)), 2, MidpointRounding.AwayFromZero);
        
        /// <summary>
        /// 总期款
        /// </summary>
        public decimal TotalInstalmentAmount => Math.Round(CreditDtoList.Sum(a => a.InstalmentList.Sum(b => b.instalmentAmount)), 2, MidpointRounding.AwayFromZero);
        
    }
}
