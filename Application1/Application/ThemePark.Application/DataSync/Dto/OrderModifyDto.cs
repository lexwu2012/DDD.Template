using System;
using System.Collections.Generic;

namespace ThemePark.Application.DataSync.Dto
{
    /// <summary>
    /// 订单修改Dto
    /// </summary>
    public class OrderModifyDto
    {
        /// <summary>
        /// 指定修改数据
        /// </summary>
        public ModifyType ModifyType { get; set; }

        /// <summary>
        /// 身份证
        /// </summary>
        public ICollection<ModifyPid> PidData { get; set; }

        /// <summary>
        /// 预订日期
        /// </summary>
        public ModifyValidStartDate ValidStartDateData { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public ICollection<ModifyPhone> PhoneData { get; set; }
    }

    /// <summary>
    /// 修改数据种类
    /// </summary>
    public enum ModifyType
    {
        /// <summary>
        /// 修改身份证
        /// </summary>
        ModifyPid = 0,


        /// <summary>
        /// 修改预订日期
        /// </summary>
        ModifyPlanDate,

        /// <summary>
        /// 修改电话
        /// </summary>
        ModifyPhone
    }


    /// <summary>
    /// 修改身份证
    /// </summary>
    public class ModifyPid
    {
        /// <summary>
        /// 新身份证
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// 子订单号
        /// </summary>
        public string TOBodyId { get; set; }

    }


    /// <summary>
    /// 修改身份证
    /// </summary>
    public class ModifyPhone
    {
        /// <summary>
        /// 新手机号
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 子订单号
        /// </summary>
        public string TOBodyId { get; set; }

    }


    /// <summary>
    /// 修改身份证
    /// </summary>
    public class ModifyValidStartDate
    {
        /// <summary>
        /// 新预订日期
        /// </summary>
        public DateTime ValidStartDate { get; set; }

        /// <summary>
        /// 主订单号
        /// </summary>
        public string TOHeaderId { get; set; }

    }

}