using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Application.Dto.User;
using DDD.Infrastructure.Web.Application;

namespace DDD.MyApplication.User.Interfaces
{
    public interface IAddUserAppService
    {
        Result AddUser(AddUserInput input);
    }
}
