using System;
using System.ComponentModel.DataAnnotations;

namespace ThemePark.Application.AliBusiness.Dto
{
    /// <summary>
    /// 请求方特系统下单数据
    /// </summary>
    [Serializable]
    public class FtOrderDetailDto
    {
        /// <summary>
        /// 购买的票种
        /// </summary>
        public string tickettypeid { get; set; }

        /// <summary>
        /// 购买票数
        /// </summary>
        public int number { get; set; }

        /// <summary>
        /// 游客身份证号码
        /// </summary>
        public string idnum { get; set; }

        /// <summary>
        /// 游客姓名
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 销售价格
        /// </summary>
        public decimal? saleprice { get; set; }
    }
}
