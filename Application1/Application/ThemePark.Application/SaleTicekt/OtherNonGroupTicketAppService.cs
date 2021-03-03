using System.Threading.Tasks;
using Abp.Domain.Repositories;
using ThemePark.Application.SaleTicekt.Interfaces;
using ThemePark.Common;
using ThemePark.Core.ParkSale;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.SaleTicekt
{
    /// <summary>
    /// 他园票业务应用层
    /// </summary>
    public class OtherNonGroupTicketAppService : ThemeParkAppServiceBase, IOtherNonGroupTicketAppService
    {
        #region Fields
        private readonly IRepository<OtherNonGroupTicket, string> _otherNonGroupTicketRepository;
        #endregion

        #region Cotr

        /// <summary>
        /// </summary>
        /// <param name="otherNonGroupTicketRepository"></param>
        public OtherNonGroupTicketAppService(IRepository<OtherNonGroupTicket, string> otherNonGroupTicketRepository)
        {
            _otherNonGroupTicketRepository = otherNonGroupTicketRepository;
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// 更改状态
        /// </summary>
        /// <param name="barCode"></param>
        /// <returns></returns>
        public async Task<Result<string>> UpdateGroupTicketToInvalidAndReturnTradeIdAsync(string barCode)
        {
            var entity = await _otherNonGroupTicketRepository.FirstOrDefaultAsync(barCode);

            if (entity != null)
            {
                entity.TicketSaleStatus = TicketSaleStatus.Refund;

                return Result.FromData(entity.TradeInfoId);
            }

            return Result.FromError<string>(ResultCode.NoRecord.DisplayName());
        }
        #endregion
    }
}
