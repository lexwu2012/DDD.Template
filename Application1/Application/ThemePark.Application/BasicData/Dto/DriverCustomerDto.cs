using Abp.AutoMapper;
using Abp.Domain.Entities.Auditing;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.BasicData.Dto
{
    /// <summary>
    /// 司机信息
    /// </summary>
    [AutoMap(typeof(DriverCustomer))]
    public class DriverCustomerDto : FullAuditedEntity
    {
        /// <summary>
        /// 代理商编号
        /// </summary>
        public int AgencyId { get; set; }
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
