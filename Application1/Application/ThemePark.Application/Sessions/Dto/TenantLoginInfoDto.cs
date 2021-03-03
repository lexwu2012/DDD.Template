using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ThemePark.Core.MultiTenancy;

namespace ThemePark.Application.Sessions.Dto
{
    [AutoMapFrom(typeof(Tenant))]
    public class TenantLoginInfoDto : EntityDto
    {
        public string TenancyName { get; set; }

        public string Name { get; set; }
    }
}