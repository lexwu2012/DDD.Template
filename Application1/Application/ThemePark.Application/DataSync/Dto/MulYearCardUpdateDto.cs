using System;
using System.Collections.Generic;
using ThemePark.Application.AgentTicket.Dto;

namespace ThemePark.Application.DataSync.Dto
{
    /// <summary>
    /// 多园年卡修改卡信息Dto
    /// </summary>
    [Serializable]
    public class MulYearCardUpdateDto
    {
        public int ParkId { get; set; }
        
        public List<CustomerDto> Customers { get; set; }

        public string IcNo { get; set; }
    }
}
