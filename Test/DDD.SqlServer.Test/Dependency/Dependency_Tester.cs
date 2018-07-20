using System.Linq;
using DDD.Domain.Core.Model.Repositories.Interfaces;
using DDD.Domain.Service.User.Interfaces;
using DDD.Infrastructure.Domain.Repositories;
using Shouldly;
using Xunit;

namespace DDD.SqlServer.Test.Dependency
{
    public class Dependency_Tester : TestBaseWithLocalIocManager
    {
        private readonly IBlogRepository _blogRepository;
        public Dependency_Tester()
        {
            //LocalIocManager.IocContainer.Register(
            //    Component.For<IUserRepository, UserRepository>().ImplementedBy<UserRepository>().LifestyleSingleton()                
            //);

            LocalIocManager.RegisterAssemblyByConvention(typeof(IRepository).Assembly);
        }

        [Fact]
        public void Test()
        {

        }

        [Fact]
        public void RepositoryTest()
        {
            var registered = LocalIocManager.IsRegistered(typeof(IUserRepository));
            var checkoffAutoRegistered = LocalIocManager.IsRegistered(typeof(ICheckoffAutoAcpRepository));

            if (checkoffAutoRegistered && registered)
            {
                //var userRepository = LocalIocManager.IocContainer.Resolve<IUserRepository>();

                //var user = userRepository.GetAllUsers().FirstOrDefault();
                //if (user != null)
                //    user.ShouldNotBeNull();

                var checkoffAuto = LocalIocManager.IocContainer.Resolve<ICheckoffAutoAcpRepository>();

                var one = checkoffAuto.GetAll().FirstOrDefault();
                one.ShouldNotBeNull();

            }

            var domainServiceRegistered = LocalIocManager.IsRegistered(typeof(IUserDomainService));
            if (domainServiceRegistered)
            {
                var userDomainService = LocalIocManager.IocContainer.Resolve<IUserDomainService>();
            }

        }
    }
}
