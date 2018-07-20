using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Domain.Service.Wechat.Dto
{
    public class InstalmentDto
    {
        string _repayDate;
        DateTime dateDue;
        bool isDate;
        public string _systemCode { get; set; }
       
        /// <summary>
        /// 合同ID
        /// </summary>
        public long contractId { get; set; }
        /// <summary>
        /// 合同号
        /// </summary>
        public long contractNo { get; set; }

        /// <summary>
        /// 合同类型
        /// </summary>
        public string creditType { get; set; }

        /// <summary>
        /// 产品类型:c-零花钱，o-非零花钱
        /// </summary>
        public string productType { get; set; }

        /// <summary>
        /// 是否有处理中的代扣记录
        /// </summary>
        public bool isWithholding { get; set; }

        /// <summary>
        /// 是否当期
        /// </summary>
        public bool IsCurrent
        {
            get
            {
                if (isDate)
                    return IsCurrentInstalmentDate(dateDue, _systemCode); //a.DateDue >= today && a.DateDue < today.AddMonths(1).Date
                else
                    return false;
            }
        }
        /// <summary>
        /// 当期是否已逾期
        /// </summary>
        public bool IsOverduce
        {
            get
            {
                if (isDate)
                    return dateDue.Date < DateTime.Today;
                else
                    return false;
            }
        }
        /// <summary>
        /// 当期欠款
        /// </summary>
        public decimal RepayAmount { get; set; }

        /// <summary>
        /// 还款日期，按指定规格格式化
        /// </summary>
        public string RepayDate
        {
            get { return _repayDate; }
            set
            {
                _repayDate = value;
                isDate = DateTime.TryParse(_repayDate, out dateDue);
            }
        }
        /// <summary>
        /// 还款日期，内部访问级不会返回给用户
        /// </summary>
        internal DateTime DueDate
        {
            get
            {
                return dateDue;
            }
        }
        /// <summary>
        /// 申请日期
        /// </summary>
        public string appDate { get; set; }
        /// <summary>
        /// 现在日期
        /// </summary>
        public string loanDate { get; set; }
        /// <summary>
        /// 当期期次
        /// </summary>
        public int numInstalment { get; set; }
        /// <summary>
        /// 分期数
        /// </summary>
        public int? paymentNum { get; set; }
        /// <summary>
        /// 当期期款
        /// </summary>
        public decimal instalmentAmount { get; set; }

        private static bool IsCurrentInstalmentDate(DateTime dateDue, string systemCode)
        {
            bool isCurrent = false;
            if (!string.IsNullOrWhiteSpace(systemCode) && systemCode.ToLower().Contains("store"))
            {//商城合同按账单显示，还款日小于等于 5号 账单日为 28号，否则账单日为 T-5 日
                isCurrent = dateDue.Day <= 5 ? dateDue.AddMonths(-1).AddDays(28 - dateDue.Day) < DateTime.Today : dateDue.AddDays(-5) < DateTime.Today;
            }
            else
            {//非商城合同按正常期显示
                isCurrent = dateDue.Date < DateTime.Today.AddMonths(1).Date;
            }

            return isCurrent;
        }
    }
}
