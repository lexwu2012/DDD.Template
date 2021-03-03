using System.Threading.Tasks;
using ThemePark.Application.DataSync.Dto;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.DataSync.Interfaces
{
    public interface IDataSyncMultiTicketAppService
    {        
        Task<Result> MultiTicketCancel(MultiTicketCancelDto dto);
        Task<Result> MultiTicketEnroll(MultiTicketEnrollDto dto);

        Task<Result> MultiTicketEnrollPhoto(MultiTicketEnrollDto dto);

        Task<Result> MultiTicketSend(MultiTicketSendDto dto);
        Task<Result> MultiTicketInpark(MultiTicketInparkDto dto);

        Result MultiTicketPhotoRemove(MultiTicketPhotoRemoveDto dto);

        /// <summary>
        /// 异步取消套票（客户端退款用）
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<Result> MultiTicketRefund(MultiTicketCancelDto dto);
    }
}
