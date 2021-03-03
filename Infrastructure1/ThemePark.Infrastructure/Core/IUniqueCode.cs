using System.Threading.Tasks;

namespace ThemePark.Infrastructure.Core
{
    public interface IUniqueCode
    {
        /// <summary>
        /// 生成唯一码
        /// </summary>
        /// <param name="codeType">唯一码的类型</param>
        /// <param name="parkId">公园Id</param>
        /// <param name="terminalId">终端Id。 codeType为<see cref="CodeType.Barcode"/>类型时需要传终端号</param>
        /// <returns>唯一码</returns>
        Task<string> CreateAsync(CodeType codeType, int parkId, int terminalId = 0);

        /// <summary>
        /// 解码并返回流水号
        /// </summary>
        /// <param name="codeType">Type of the code.</param>
        /// <param name="code">The code.</param>
        int DecodeFlow(CodeType codeType, string code);
    }
}
