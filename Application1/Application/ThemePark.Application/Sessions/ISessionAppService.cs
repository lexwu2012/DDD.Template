using System.Threading.Tasks;
using Abp.Application.Services;
using ThemePark.Application.Sessions.Dto;

namespace ThemePark.Application.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
