using System.Collections.Generic;
using System.Threading.Tasks;
using DDD.Domain.Core.Model.Repositories.Dto;
using DDD.Infrastructure.Domain.Application;
using DDD.Infrastructure.Web.Application;
using DDD.Infrastructure.Web.Query;

namespace DDD.Application.Service.User.Interfaces
{
    public interface IUserAppService : IApplicationService
    {
        Result<int> AddUser(AddUserInput input);

        Result<int> GetAllUsers();

        Result<UserDto> UpdateSpecifyUser(int id);

        /// <summary>
        /// 获取单个用户信息
        /// </summary>
        Task<TDto> GetUserAsync<TDto>(IQuery<Domain.Core.Model.User> query);
        
        /// <summary>
        /// 获取用户信息列表
        /// </summary>
        Task<IList<TDto>> GetAgencySaleTicketClassListAsync<TDto>(IQuery<Domain.Core.Model.User> query);
    }
}
