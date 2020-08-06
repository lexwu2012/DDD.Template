using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Application.Dto.User;
using DDD.Application.Service.User.Interfaces;
using DDD.Domain.Core;
using DDD.Domain.Core.DbContextRelate;
using DDD.Domain.Core.Repositories;
using DDD.Infrastructure.Ioc.Dependency;
using DDD.Test;
using Shouldly;
using Xunit;

namespace DDD.SqlServer.Test.UserService
{
    public class User_Service_Tests : TestBaseWithLocalIocManager
    {
        public User_Service_Tests()
        {
            //注册dbcontext中的实体泛型仓储
            using (var repositoryRegistrar = LocalIocManager.ResolveAsDisposable<EfGenericRepositoryRegistrar>())
            {
                repositoryRegistrar.Object.RegisterForDbContext(typeof(DDDDbContext), LocalIocManager, EfAutoRepositoryTypes.Default);
            }
        }

        [Fact]
        public async Task User_Service_Query_Should_Be_Correctly()
        {
            var userService = LocalIocManager.IocContainer.Resolve<IUserAppService>();

            var result = await userService.GetUsersAsync<UserDto>(new UserQuery());

            result.Data.Count().ShouldBeGreaterThanOrEqualTo(4);
        }
    }
}
