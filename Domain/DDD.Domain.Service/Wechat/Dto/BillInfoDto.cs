using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Domain.Service.Wechat.Dto
{
    public class BillInfoDto
    {
        string _systemCode = null;
        private DateTime _repayDate;
        public BillInfoDto(List<InstalmentDto> _contractList, string systemCode, DateTime? repayDate = null)
        {
            _systemCode = systemCode;
            if ((_contractList == null || _contractList.Count == 0) && repayDate == null)
            {
                throw new Exception("初始化账单发生异常！");
            }

            if (_contractList == null)
            {
                _contractList = new List<InstalmentDto>();
            }
            contractList = _contractList;

            if (_contractList == null || _contractList.Count == 0)
            {
                _repayDate = repayDate.Value;
            }
            else
            {
                _repayDate = contractList.Min(a => a.DueDate);
            }
        }
        /// <summary>
        /// 合同列表
        /// </summary>
        public List<InstalmentDto> contractList { get; set; }
        ///// <summary>
        ///// 是否当期账单
        ///// </summary>
        //public bool isCurrent { get { return IsCurrentInstalmentDate(_repayDate, _systemCode); } }
        ///// <summary>
        ///// 账单是否已逾期
        ///// </summary>
        //public bool isOverduce { get { return contractList.Any(a => a.isOverduce); } }
        /// <summary>
        /// 是否存在正在处理的代扣记录
        /// </summary>
        public bool isWithholding { get { return contractList.Any(a => a.isWithholding); } }

        /// <summary>
        /// 当期账单总期款
        /// </summary>
        public decimal totalInstalmentAmount { get { return contractList.Sum(a => a.instalmentAmount); } }

        /// <summary>
        /// 当期账单总欠款
        /// </summary>
        public decimal repayAmount { get { return contractList.Sum(a => a.RepayAmount); } }

        /// <summary>
        /// 账单日期
        /// </summary>
        public string repayBillDate => _repayDate.ToString("yyyy/MM/dd");
    }
}
