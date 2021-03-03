using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.BasicData.Dto
{
    /// <summary>
    /// 
    /// </summary>
    [AutoMap(typeof(Terminal))]
    public class AddTerminalInput
    {
        #region Properties

        /// <summary>
        /// 终端号
        /// </summary>
        [Required]
        [Range(1, 255)]
        [Display(Name = "终端号")]
        public short TerminalCode { get; set; }

        /// <summary>
        /// 终端名称
        /// </summary>
        [StringLength(20)]
        [Display(Name = "终端名称")]
        public string TerminalName { get; set; }

        /// <summary>
        /// 当前序列号
        /// </summary>
        public int? CurSerialNo { get; set; }

        /// <summary>
        /// 当前交易号
        /// </summary>
        public int? CurTradeNo { get; set; }

        /// <summary>
        /// 主机名称
        /// </summary>
        [StringLength(50)]
        [Display(Name = "主机名称")]
        public string HostName { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        [StringLength(50)]
        [Display(Name = "IP地址")]
        public string Ip { get; set; }

        /// <summary>
        /// mac地址
        /// </summary>
        [StringLength(50)]
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

        /// <summary>
        /// 公园编号
        /// </summary>
        public int? ParkId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(50)]
        public string Remark { get; set; }



        #endregion Properties
    }
}
