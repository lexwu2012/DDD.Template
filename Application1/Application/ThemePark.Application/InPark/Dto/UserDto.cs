using Abp.AutoMapper;
using ThemePark.Core.Authorization.Users;

namespace ThemePark.Application.InPark.Dto
{
    /// <summary>
    /// 入园单
    /// </summary>
    [AutoMap(typeof(User))]
    public class UserDto
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; }


    }
}
