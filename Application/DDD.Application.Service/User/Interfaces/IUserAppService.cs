using DDD.Domain.Core.Model.Repositories.Dto;
using DDD.Infrastructure.Domain.Application;
using DDD.Infrastructure.Web.Application;

namespace DDD.Application.Service.User.Interfaces
{
    public interface IUserAppService : IApplicationService
    {
        Result<int> AddUser(AddUserInput input);

        Result<int> GetAllUsers();

    }
}
