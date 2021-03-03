using Abp.AutoMapper;
using ThemePark.Core.BasicData;

namespace ThemePark.Application.BasicData.Dto
{
    [AutoMap(typeof(Terminal))]
    public class GetTerninalIdInput
    {
        /// <summary>
        /// 主机IP
        /// </summary>
        public string Ip { get; set; }
    }
}
