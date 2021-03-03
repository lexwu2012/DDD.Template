using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.BasicData.Dto
{
    /// <summary>
    /// 查询
    /// </summary>
    public class SearchTerminalInput
    {
        /// <summary>
        /// IP地址
        /// </summary>
        [Query(QueryCompare.Like, nameof(Core.BasicData.Terminal.Ip))]
        public string Ip { get; set; }

        /// <summary>
        /// 终端号
        /// </summary>
        [Query(QueryCompare.Like, nameof(Core.BasicData.Terminal.TerminalCode))]
        public short TerminalCode { get; set; }
    }
}
