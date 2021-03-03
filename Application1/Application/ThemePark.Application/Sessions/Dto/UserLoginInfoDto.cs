using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ThemePark.Core.Authorization.Users;

namespace ThemePark.Application.Sessions.Dto
{
    [AutoMapFrom(typeof(User))]
    public class UserLoginInfoDto : EntityDto<long>
    {
        public string Name { get; set; }
        
        public string UserName { get; set; }

        public string EmailAddress { get; set; }
    }
}
