using Abp.AutoMapper;
using ThemePark.ApplicationDto.BasicData;
using ThemePark.Core.BasicData;
using ThemePark.Infrastructure.Web.Models;

namespace ThemePark.Application.BasicData.Dto
{
    /// <summary>
    /// 
    /// </summary>
    [AutoMapFrom(typeof(Terminal))]
    public class TerminalDto : AuditedModel<int>
    {
        /// <summary>
        /// 终端号
        /// </summary>
        public short TerminalCode { get; set; }

        /// <summary>
        /// 当前序列号
        /// </summary>
        public int? CurSerialNo { get; set; }

        /// <summary>
        /// 当前交易号
        /// </summary>
        public int? CurTradeNo { get; set; }

        /// <summary>
        /// 主机名
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// mac地址
        /// </summary>
        public string Mac { get; set; }

        /// <summary>
        /// 终端类型： PC机 闸机 自助售取票终端
        /// </summary>
        public int? MachineType { get; set; }

        /// <summary>
        /// 最大序列号
        /// </summary>
        public int? MaxSerialNo { get; set; }

        /// <summary>
        /// 最小序列号
        /// </summary>
        public int? MinSerialNo { get; set; }

        public ParkDto Park { get; set; }

        /// <summary>
        /// 公园编号
        /// </summary>
        public int? ParkId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
