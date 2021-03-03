using Abp.AutoMapper;
using ThemePark.Core.BasicTicketType;

namespace ThemePark.Application.BasicTicketType.Dto
{
    /// <summary>
    /// 基础票种新增输入
    /// </summary>
    [AutoMap(typeof(TicketType))]
    public class TicketTypeSortInput
    {
        /// <summary>
        /// 票种编号
        /// </summary>
        public string Id { get; set; }
    
        /// <summary>
        /// 默认排序
        /// </summary>
        public int SerialNumber { get; set; }
    }
}
