using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.DataSync.Dto
{
    /// <summary>
    /// 多园年卡激活Dto
    /// </summary>
    [Serializable]
    public class MulYearCardActiveDto
    {
        public long VipCardId { get; set; }

        public int ParkId { get; set; }

        /// <summary>
        /// 促销票类编号
        /// </summary>    
        public int? ParkSaleTicketClassId { get; set; }

        /// <summary>
        /// 开始有效日期
        /// </summary>    
        public DateTime? ValidDateBegin { get; set; }

        /// <summary>
        /// 结束有效日期
        /// </summary>    
        public DateTime? ValidDateEnd { get; set; }

        /// <summary>
        /// 终端号GetUsersRoleNamesAsync
        /// </summary>    
        public int? TerminalId { get; set; }


        /// <summary>
        /// 激活人
        /// </summary>
        public long? ActiveUser { get; set; }

        /// <summary>
        /// 激活时间
        /// </summary>
        public DateTime? ActiveTime { get; set; }


        //电子年卡初始化

            /// <summary>
            /// 是否电子年卡
            /// </summary>
        public bool IsEcard { get; set; }

        /// <summary>
        /// Ic卡内码
        /// </summary>    
        public string IcNo { get; set; }
        /// <summary>
        /// IC卡面编号
        /// </summary>    

        public string CardNo { get; set; }

        /// <summary>
        /// 电子卡面编号
        /// </summary>    

        public string ECardID { get; set; }

        /// <summary>
        /// 类型编号
        /// </summary>    

        public int KindId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long IcBasicInfoId { get; set; }

 

        /// <summary>
        /// 基础票类Id(年卡类型)
        /// </summary>
        public int TicketClassId { get; set; }


        public long? CreatorUserId { get; set; }











        public List<MulCustomerDto> Customers { get; set; }

        public List<MulUserIcDto> UserICs { get; set; }

    }


    /// <summary>
    /// 顾客表
    /// </summary>
    public class MulCustomerDto : FullAuditedEntity<long>
    {
        #region Properties



        /// <summary>
        /// 条码号
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// 公园ID
        /// </summary>
        public int? ParkId { get; set; }

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
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        #endregion Properties
    }


    [Serializable]
    public class MulUserIcDto
    {
        public long Id { get; set; }
        /// <summary>
        /// 客户编号
        /// </summary>    
        public long CustomId { get; set; }

        /// <summary>
        /// Ic卡编号
        /// </summary>    
        public long? IcBasicInfoId { get; set; }

        /// <summary>
        /// Ic卡编号
        /// </summary>    
        public long? VIPCardId { get; set; }


        public string Remark { get; set; }

    }


}
