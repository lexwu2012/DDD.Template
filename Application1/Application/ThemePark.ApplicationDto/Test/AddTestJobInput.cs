using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using ThemePark.Core.Test;

namespace ThemePark.ApplicationDto.Test
{
    [AutoMapTo(typeof(TestJob))]
    public class AddTestJobInput
    {
        #region Properties
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        //[Required, Range(1, long.MaxValue)]
        public long UserId { get; set; }
        #endregion
    }
}
