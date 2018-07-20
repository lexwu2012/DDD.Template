using System.Linq;
using DDD.Domain.Core.Model.Repositories.Interfaces;
using DDD.Domain.Service.User.Interfaces;
using DDD.Infrastructure.Domain.Repositories;
using Shouldly;
using Xunit;

namespace DDD.Oracle.Test.Dependency
{
    public class Dependency_Tester : OracleTestBaseWithLocalIocManager
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
            var checkoffAutoRegistered = LocalIocManager.IsRegistered(typeof(ICheckoffAutoAcpRepository));

            if (checkoffAutoRegistered)
            {
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
