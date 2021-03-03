using System.Threading.Tasks;
using ThemePark.Infrastructure.Application;
using ThemePark.VerifyTicketDto.Dto;

namespace ThemePark.Application.VerifyTicket
{
    /// <summary>
    /// 验票策略接口
    /// </summary>
    public interface ICheckStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="terminal"></param>
        /// <returns></returns>
        Task<Result<VerifyDto>> Verify(string barcode, int terminal);
    }
}