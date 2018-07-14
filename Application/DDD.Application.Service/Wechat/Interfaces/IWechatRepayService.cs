using System.Threading.Tasks;
using DDD.Domain.Common.Application;
using DDD.Domain.Service.Wechat.Dto;
using DDD.Infrastructure.Web.Application;

namespace DDD.Application.Service.Wechat.Interfaces
{
    public interface IWechatRepayService: IApplicationService
    {
        Result<PayInfoDto> GetPayInfoAsync(GetPayInfoDto payInfo);

        Task<Result<int>> GetTodayAutoCheckoffTotalAsync();

        Result<int> GetConstantData();
    }
}
