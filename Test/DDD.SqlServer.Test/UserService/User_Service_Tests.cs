using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public void User_Service_Should_Be_Resolve_Correctly()
        {
            var userService = LocalIocManager.IocContainer.Resolve<IUserAppService>();

            userService.GetAllUsers().Data.ShouldBeGreaterThanOrEqualTo(4);
        }

        [Fact]
        public void User_Should_Be_Update_Correctly()
        {
            var userService = LocalIocManager.IocContainer.Resolve<IUserAppService>();

            userService.GetAllUsers().Data.ShouldBeGreaterThanOrEqualTo(4);
        }
    }
}
