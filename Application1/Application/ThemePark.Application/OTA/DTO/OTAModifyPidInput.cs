namespace ThemePark.Application.OTA.DTO
{
    /// <summary>
    /// 修改身份证
    /// </summary>
    public class ModifyPidInput:ValidateParams
    {
        /// <summary>
        /// 第三方订单号
        /// </summary> 
        public string AgentOrderId { get; set; }

        /// <summary>
        /// 新身份证
        /// </summary>
   
        public string Pid { get; set; }

        /// <summary>
        /// 旧身份证
        /// </summary>
        public string OldPid { get; set; }

        /// <summary>
        /// 代理商ID
        /// </summary>
        public int AgencyId { get; set; }
    }
}
