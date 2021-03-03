
namespace ThemePark.Application.Agencies.Dto
{
    /// <summary>
    /// 组合类型
    /// </summary>
    public class AgencyCombineDto
    {
        /// <summary>
        /// 代理商Id
        /// </summary>
        public int AgencyId { get; set; }

        /// <summary>
        /// 复选框状态
        /// </summary>
        public bool Checked { get; set; }

        /// <summary>
        /// 代理商名称
        /// </summary>
        public string AgencyName { get; set; }

        /// <summary>
        /// 代理商所属 省份
        /// </summary>
        public string ProvinceName { get; set; }

        /// <summary>
        /// 代理商所属城市
        /// </summary>
        public string CityName { get; set; }
    }
}
