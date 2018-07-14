using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Domain.Service.Wechat.Dto
{
    public class PayInfoDto
    {

        public ImportantInfoDto Important { get; set; }
        public ProductInfoDto product { get; set; }

        public PayInfoDto()
        {
            Important = new ImportantInfoDto
            {
                GetEndDate = () =>
                {
                    List<string> dateList = new List<string>();
                    if (product.cycleMoney.ContractList.Count > 0)
                        dateList.AddRange(product.cycleMoney.ContractList.Select(a => a.RepayDate));
                    if (product.notCycleMoney.CreditDtoList.Count > 0)
                        dateList.AddRange(
                            product.notCycleMoney.CreditDtoList.Select(a => a.InstalmentList.Min(i => i.RepayDate)));

                    if (dateList.Count > 0)
                        return dateList.Min(a => a);
                    else
                        return "";
                },
                GetIsOverDuce = () =>
                {
                    bool isOverDuce = false;
                    if (product.cycleMoney.ContractList.Count > 0)
                        isOverDuce = isOverDuce || product.cycleMoney.ContractList.Any(a => a.IsOverduce);
                    if (product.notCycleMoney.CreditDtoList.Count > 0)
                        isOverDuce = isOverDuce ||
                                     product.notCycleMoney.CreditDtoList.Any(a => a.InstalmentList.Any(b => b.IsOverduce));
                    return isOverDuce;
                },
                GetMaxRepayAmount = () =>
                {
                    decimal maxRepayAmount = 0;
                    maxRepayAmount += product.cycleMoney.MaxRepayAmount;
                    maxRepayAmount += product.notCycleMoney.MaxRepayAmount;
                    return maxRepayAmount;
                },
                GetCycleTotalAmount = () => { return product.cycleMoney.MaxRepayAmount; },
                GetRepayAmount = () =>
                {
                    decimal repayAmount = 0;
                    repayAmount += product.cycleMoney.RepayAmount;
                    repayAmount += product.notCycleMoney.RepayAmount;
                    return repayAmount;
                },
                GetInstalmentAmount = () =>
                {
                    decimal instalmentAmount = 0;
                    instalmentAmount += product.cycleMoney.TotalAmount;
                    instalmentAmount += product.notCycleMoney.InstalmentAmount;
                    return instalmentAmount;
                },
                GetTotalInstalmentAmount = () =>
                {
                    decimal instalmentAmount = 0;
                    instalmentAmount += product.cycleMoney.TotalAmount;
                    instalmentAmount += product.notCycleMoney.InstalmentAmount;
                    return instalmentAmount;
                }
            };
        }
    }
}
