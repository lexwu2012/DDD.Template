using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Domain.Service.Wechat.Dto
{
    public class ProductInfoDto
    {
        string _systemCode = null;
        internal void SetSystemCode(string systemCode)
        {
            cycleMoney = new CycleInfoDto();
            notCycleMoney = new CreditInfoDto();
            _systemCode = systemCode;

        }
        /// <summary>
        /// 零花钱借款信息
        /// </summary>
        public CycleInfoDto cycleMoney { get; set; }

        /// <summary>
        /// 非零花钱借款信息
        /// </summary>
        internal CreditInfoDto notCycleMoney { get; set; }

        /// <summary>
        /// 非零花钱账单，使用 notCycleMoney 转换而来
        /// </summary>
        public List<BillInfoDto> billList
        {
            get
            {
                List<BillInfoDto> list = new List<BillInfoDto>();
                if (notCycleMoney.CreditDtoList != null && notCycleMoney.CreditDtoList.Count > 0)
                {//非零花钱有欠款
                    var instalmentList = new List<InstalmentDto>();
                    notCycleMoney.CreditDtoList.ForEach(a => instalmentList.AddRange(a.InstalmentList));
                    var currentInstalmentList = instalmentList.FindAll(a => a.IsCurrent);
                    if (currentInstalmentList == null || currentInstalmentList.Count == 0)
                    {//当期已还清
                        var minDate = instalmentList.Min(a => a.DueDate);
                        while (true)
                        {//添加已还清的空账单，可能存在还未到期的也已经还款清
                            minDate = minDate.AddMonths(-1);
                            list.Insert(0, new BillInfoDto(null, _systemCode, minDate));
                            //if (IsCurrentInstalmentDate(minDate, _systemCode))
                            //    break;
                        }
                    }
                    else
                    {//当期未还清
                        list.Add(new BillInfoDto(currentInstalmentList, _systemCode));
                    }
                    instalmentList = instalmentList.Except(currentInstalmentList).ToList();
                    //按账单日期，依次添加账单到账单列表
                    var dueGroup = instalmentList.GroupBy(a => a.DueDate).OrderBy(a => a.Key);
                    foreach (var group in dueGroup)
                    {
                        list.Add(new BillInfoDto(group.ToList(), _systemCode));
                    }
                }
                return list;
            }
        }

        public void Set_notCycleMoney_isWithholding(bool isWithholding)
        {
            notCycleMoney.IsWithholding = isWithholding;
        }

        public void Add_notCycleMoneyContract(CreditDto credit)
        {
            notCycleMoney.CreditDtoList.Add(credit);
        }
    }
}
