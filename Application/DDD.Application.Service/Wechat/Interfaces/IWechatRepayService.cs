using System.Threading.Tasks;
using DDD.Domain.Service.Wechat.Dto;
using DDD.Infrastructure.Domain.Application;
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
