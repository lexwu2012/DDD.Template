using System.Data.Common;
using Castle.MicroKernel.Registration;
using DDD.Domain.Core.Model;
using DDD.Domain.Core.Model.Repositories.Interfaces;
using DDD.Domain.Core.Model.ValueObj;
using Shouldly;
using Xunit;

namespace DDD.Test.EF
{
    public class SpecifiedObjCrudTest : IntegratedTestBase
    {
        private readonly IUserRepository _userRepository;
        

        public SpecifiedObjCrudTest()
        {
            _userRepository = LocalIocManager.IocContainer.Resolve<IUserRepository>();


            LocalIocManager.IocContainer.Register(
                Component.For<DbConnection>()
                    .UsingFactoryMethod(Effort.DbConnectionFactory.CreateTransient)
                    .LifestyleSingleton()
                );
        }

        [Fact]
        public void ObjAddTest()
        {
            var user = new User
            {
                Address = new Address { Street = "福田路" },
                Name = "lex"
            };

            var result = _userRepository.Insert(user);

            result.ShouldNotBeNull();
        }
    }
}
