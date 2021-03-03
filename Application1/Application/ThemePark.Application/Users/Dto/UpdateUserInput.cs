using Abp.AutoMapper;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ThemePark.Core.Authorization.Users;

namespace ThemePark.Application.Users.Dto
{
    /// <summary>
    /// 更新代理商用户
    /// </summary>
    [AutoMap(typeof(User))]
    public class UpdateUserInput
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 登录名
        /// </summary>
        public string UserName { get; set; }
    }
}
