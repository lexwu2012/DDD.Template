using Abp.AutoMapper;
using System;
using System.ComponentModel.DataAnnotations;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.AgentTicket.Dto
{
    [AutoMapTo(typeof(Customer))]
    public class CustomerInput
    {
        /// <summary>
        /// 顾客姓名
        /// </summary>
        [StringLength(20)]
        public string CustomerName { get; set; }

        /// <summary>
        /// 指纹模板1
        /// </summary>
        public System.Byte[] Fp1 { get; set; }

        /// <summary>
        /// 指纹模板2
        /// </summary>
        public System.Byte[] Fp2 { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public GenderType GenderType { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Mail { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 相片
        /// </summary>
        public System.Byte[] Photo { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// QQ号
        /// </summary>
        public string Qq { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        [StringLength(100)]
        public string Address { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime Birthday =>DateTime.Now;

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public CustomerStateType CustomerStateType { get; set; }

        /// <summary>
        /// 微信账号
        /// </summary>
        public string Weixin { get; set; }
    }
}
