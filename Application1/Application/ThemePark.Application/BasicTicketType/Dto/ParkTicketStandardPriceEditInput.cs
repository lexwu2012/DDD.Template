using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThemePark.Core.BasicData;
using ThemePark.Core.BasicTicketType;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.BasicTicketType.Dto
{
    /// <summary>
    /// 门市价更改模型
    /// </summary>
    [AutoMap(typeof(ParkTicketStandardPrice))]
    public class ParkTicketStandardPriceEditInput : ParkTicketStandardPriceUpdateinput
    {
        /// <summary>
        /// 公园名称
        /// </summary>
        [DisplayName("公园")]
        [MapFrom(nameof(ParkTicketStandardPrice.Park), nameof(Park.ParkName))]
        public string ParkName { get; set; }

        /// <summary>
        /// 票种名称
        /// </summary>
        [DisplayName("票种名称")]
        [MapFrom(nameof(ParkTicketStandardPrice.TicketType), nameof(TicketType.TicketTypeName))]
        public string TicketTypeName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DisplayName("备注")]
        public string Remark { get; set; }
    }
}
