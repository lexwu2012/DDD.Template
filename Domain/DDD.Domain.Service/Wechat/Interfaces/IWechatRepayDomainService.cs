using System.Threading.Tasks;
using DDD.Domain.Common.Application;
using DDD.Domain.Service.Wechat.Dto;
using DDD.Infrastructure.Web.Application;

namespace DDD.Domain.Service.Wechat.Interfaces
{
    public interface IWechatRepayDomainService : IDomainService
    {
        Result<PayInfoDto> GetPayInfo(GetPayInfoDto payInfoDto);

        Task<Result<int>> GetTodayAutoCheckoffTotalAsync();

        Result<int> GetConstantData();
    }
}
