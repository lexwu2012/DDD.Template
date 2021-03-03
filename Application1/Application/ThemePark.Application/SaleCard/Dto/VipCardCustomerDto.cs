﻿using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThemePark.Common;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.SaleCard.Dto
{
    /// <summary>
    /// 
    /// </summary>
    [AutoMap(typeof(Customer))]
    public class VipCardCustomerDto
    {
        /// <summary>
        /// 顾客姓名
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 指纹模板1 TXW
        /// </summary>
        public Byte[] Fp1 { get; set; }

        /// <summary>
        /// 指纹模板2 ZK
        /// </summary>
        public Byte[] Fp2 { get; set; }

        /// <summary>
        /// 指纹图片1 txw
        /// </summary>
        public Byte[] FpImage1 { get; set; }

        /// <summary>
        /// 指纹图片2 zk
        /// </summary>
        public Byte[] FpImage2 { get; set; }


        /// <summary>
        /// 性别说明
        /// </summary>
        public string GenderTypeName => GenderType.DisplayName();

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
        public Byte[] Photo { get; set; }

        /// <summary>
        /// 人脸特征点
        /// </summary>
        /// <value>The photo feature.</value>
        public byte[] PhotoFeature { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public string BirthdayStr => Birthday.Value.ToString("yyyy-MM-dd");
    }
}
