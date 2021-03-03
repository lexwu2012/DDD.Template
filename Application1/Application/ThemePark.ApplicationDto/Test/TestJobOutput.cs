using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ThemePark.ApplicationDto.Users;
using ThemePark.Core.Test;

namespace ThemePark.ApplicationDto.Test
{
    [AutoMapFrom(typeof(TestJob))]
    public class TestJobDto : EntityDto
    {
        #region Properties
        public string Name { get; set; }

        public long UserId { get; set; }

        public virtual UserListDto User { get; set; }
        #endregion
    }
}
