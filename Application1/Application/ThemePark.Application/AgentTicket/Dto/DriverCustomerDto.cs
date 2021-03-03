using Abp.AutoMapper;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.AgentTicket.Dto
{
    /// <summary>
    /// 司机信息
    /// </summary>
    [AutoMap(typeof(DriverCustomer))]
    public class DriverCustomerDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string Idcard { get; set; }
    }
}
