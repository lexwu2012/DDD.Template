using Abp.AutoMapper;
using ThemePark.Core.CardManage;

namespace ThemePark.Application.SaleCard.Dto
{
    /// <summary>
    /// 卡初始化信息
    /// </summary>
    [AutoMap(typeof(IcBasicInfo))]
    public  class IcBasicInfoDto 
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Ic卡内码
        /// </summary>    
        public string IcNo { get; set; }
        /// <summary>
        /// IC卡面编号
        /// </summary>    

        public string CardNo { get; set; }


        /// <summary>
        /// 电子年卡编号
        /// </summary>
        public string ECardID { get; set; }

        /// <summary>
        /// 类型编号
        /// </summary>    
        public int KindId { get; set; }
        /// <summary>
        /// 备注
        /// </summary>    
        public string Remark { get; set; }

        /// <summary>
        /// 卡类别
        /// </summary>
        public virtual ICKindDto KindICKind { get; set; }
    }
}
