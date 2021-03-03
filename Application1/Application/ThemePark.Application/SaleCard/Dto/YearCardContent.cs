using System;
using System.Collections.Generic;

namespace ThemePark.Application.SaleCard.Dto
{
    /// <summary>
    /// 年卡打印参数类
    /// </summary>
    public class YearCardContent
    {
        /// <summary>
        /// 模板Id
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// 人数
        /// </summary>
        public int Persons { get; set; }

        /// <summary>
        /// 用户信息列表
        /// </summary>
        public List<YearCardUserInfo> YearCardUserInfo { get; set; }

        /// <summary>
        /// 年卡有效期
        /// </summary>
        public DateTime ValidDate { get; set; }

        /// <summary>
        /// 年卡卡面编号
        /// </summary>
        public string CardNo { get; set; }

    }
    /// <summary>
    /// 
    /// </summary>
    public class YearCardUserInfo
    {
        /// <summary>
        /// 游客姓名
        /// </summary>
        public string CustomName { get; set; }

        /// <summary>
        /// 照片
        /// </summary>
        public byte[] Photo { get; set; }
    }

}
