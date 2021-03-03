using Abp.AutoMapper;
using ThemePark.Core.BasicTicketType;

namespace ThemePark.Application.BasicTicketType.Dto
{
    /// <summary>
    /// 基础票种新增输入
    /// </summary>
    [AutoMap(typeof(TicketType))]
    public class TicketTypeAddNewInput
    {
        public string OriginalId { get; set; }

        /// <summary>
        /// 票种编号
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 票类名称 成人 儿童 有年龄和身高限制 长者 有年龄限制 大学生 有学生证限制 中高考毕业生 有准考证限制 中学生。。。 小学生。。。
        /// </summary>    
        public string TicketTypeName { get; set; }

        /// <summary>
        /// 是否有入园限制
        /// </summary>    
        public bool? IsLimited { get; set; }

        /// <summary>       
        /// Remark
        /// </summary>    
        public string Remark { get; set; }

        /// <summary>
        /// 默认排序
        /// </summary>
        public int SerialNumber { get; set; }

        /// <summary>
        /// 人数
        /// </summary>
        public int Persons { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
