using System.Threading.Tasks;
using ThemePark.Application.DataSync.Dto;
using ThemePark.Infrastructure.Application;

namespace ThemePark.Application.DataSync.Interfaces
{

    /// <summary>
    /// 
    /// </summary>
    public interface IDataSyncMulYearCardAppService
    {

        Task<Result> MulYearCardActive(MulYearCardActiveDto dto);
        Task<Result> MulYearCardFill(MulYearCardFillDto dto);

        Task<Result> MulYearCardInit(MulYearCardInitDto dto);

        Task<Result> MulYearCardLoss(MulYearCardLossDto dto);
        Task<Result> MulYearCardRenew(MulYearCardRenewDto dto);

        Task<Result> MulYearCardSale(MulYearCardSaleDto dto);
        Task<Result> MulYearCardUnLoss(MulYearCardUnLossDto dto);
        Task<Result> MulYearCardUpdate(MulYearCardUpdateDto dto);

        Task<Result> MulYearCardVoucherReturn(MulYearCardVoucherReturnDto dto);
        Task<Result> MulYearCardVoucherSale(MulYearCardVoucherSaleDto dto);
        Task<Result> MulYearCardReturn(MulYearCardReturnDto dto);

    }
}
