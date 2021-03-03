using Abp.AutoMapper;
using ThemePark.Core.CardManage;

namespace ThemePark.Application.SaleCard.Dto
{
    /// <summary>
    /// IC卡类别
    /// </summary>
    [AutoMap(typeof(ICKind))]
    public  class ICKindDto
    {
        /// <summary>
        /// 类型名称
        /// </summary>   
        public string KindName { get; set; }
        /// <summary>
        /// 是否二次入园管理卡
        /// </summary>    
        public bool IsTimeCard { get; set; }
        /// <summary>
        /// 是否管理卡
        /// </summary>    
        public bool IsManageCard { get; set; }
        /// <summary>
        /// 有效天数
        /// </summary>    
        public int? ValidDays { get; set; }
        /// <summary>
        /// 备注
        /// </summary>    
        public string Remark { get; set; }
    }
}
