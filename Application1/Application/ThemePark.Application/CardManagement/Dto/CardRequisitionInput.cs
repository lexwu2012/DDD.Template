using Abp.AutoMapper;
using ThemePark.Core.CardManage;

namespace ThemePark.Application.CardManagement.Dto
{
    /// <summary>
    /// 管理卡领用输入
    /// </summary>
    [AutoMap(typeof(ManageCardInfo))]
    public class CardRequisitionInput
    {
        /// <summary>
        /// 所属人
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 所属部门
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// Ic卡编号
        /// </summary>    
        public int IcBasicInfoId { get; set; }

        /// <summary>
        /// 开始有效日期
        /// </summary>    
        public System.DateTime? ValidDateBegin { get; set; }

        /// <summary>
        /// 结束有效日期
        /// </summary>    
        public System.DateTime? ValidDateEnd { get; set; }
    }
}
