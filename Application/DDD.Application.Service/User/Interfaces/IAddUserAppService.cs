using DDD.Application.Dto.User;
using DDD.Infrastructure.Web.Application;

namespace DDD.Application.Service.User.Interfaces
{
    public interface IAddUserAppService : IApplicationService
    {
        Result AddUser(AddUserInput input);

    }
}
