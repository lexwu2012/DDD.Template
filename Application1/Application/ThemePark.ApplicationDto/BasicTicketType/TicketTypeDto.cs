using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ThemePark.Core.BasicTicketType;

namespace ThemePark.ApplicationDto.BasicTicketType
{

    [AutoMap(typeof(TicketType))]
    public class TicketTypeDto : FullAuditedEntityDto
    {
        /// <summary>
        /// 票种类型名称
        /// </summary>    
        public string TicketTypeName { get; set; }

        /// <summary>
        /// 人数
        /// </summary>
        public int Persons { get; set; }

        /// <summary>
        /// 是否特殊要求
        /// </summary>    
        public bool IsLimited { get; set; }

        /// <summary>
        /// 排序序号
        /// </summary>
        public int SerialNumber { get; set; }

        /// <summary>
        /// 备注
        /// </summary>    
        public string Remark { get; set; }

        /// <summary>
        /// 业务主键
        /// </summary>
        public string TicketTypeCode { get; set; }

    }


    [AutoMap(typeof(TicketType))]
    public class TicketTypeNewDto 
    {
        /// <summary>
        /// 人数
        /// </summary>
        public int Persons { get; set; }

    }

}

