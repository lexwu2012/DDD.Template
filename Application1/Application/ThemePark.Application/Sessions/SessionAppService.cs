using System.Threading.Tasks;
using Abp.Auditing;
using Abp.AutoMapper;
using ThemePark.Application.Sessions.Dto;

namespace ThemePark.Application.Sessions
{
    public class SessionAppService : ThemeParkAppServiceBase, ISessionAppService
    {
        [DisableAuditing]
        public async Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations()
        {
            var output = new GetCurrentLoginInformationsOutput
            {
                User = (await GetCurrentUserAsync()).MapTo<UserLoginInfoDto>()
            };
            
            return output;
        }
    }
}