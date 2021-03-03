using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using ThemePark.ApplicationDto.BasicData;
using ThemePark.Core.BasicTicketType;
using ThemePark.Infrastructure.Application;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.BasicTicketType.Dto
{
    /// <summary>
    /// 门市价可更改字段保存模型
    /// </summary>
    [AutoMap(typeof(ParkTicketStandardPrice))]
    public class ParkTicketStandardPriceUpdateinput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 门市价
        /// </summary>
        [DisplayName("门市价")]
        [Required(ErrorMessage = "必填字段")]
        [RegularExpression(@"^(0|[1-9][0-9]{0,9})(\.[0-9]{1,2})?$", ErrorMessage = "输入必须为大于0的数字,最多保留两位小数")]
        public decimal StandardPrice { get; set; }
    }
}
