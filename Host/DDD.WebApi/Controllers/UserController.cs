

using DDD.Application.Dto.User;
using DDD.Application.Service.User.Interfaces;
using DDD.Infrastructure.Web.Application;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyWebApi.Controllers
{
    public class UserController : ApiControllerBase
    {

        public readonly IUserAppService _userAppService;

        public UserController(IUserAppService userAppService)
        {
            _userAppService = userAppService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<Result<UserDto>> GetUser(UserQuery query)
        {
            return await _userAppService.GetUserAsync<UserDto>(query);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<Result<List<UserDto>>> GetUsers(UserQuery query)
        {
            return await _userAppService.GetUsersAsync<UserDto>(query);
        }
    }
}