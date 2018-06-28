using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using DDD.Domain.Common.Repositories;
using DDD.Domain.Core.Model;
using DDD.Domain.Core.Model.Repositories;
using DDD.Domain.Core.Model.Repositories.Interfaces;
using DDD.Domain.Core.Model.ValueObj;
using DDD.Domain.Core.Repositories;
using DDD.DomainService.DomainService.Interfaces;
using Shouldly;
using Xunit;

namespace DDD.Test.Dependency
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

            if (registered)
            {
                var userRepository = LocalIocManager.IocContainer.Resolve<IUserRepository>();

                var user = userRepository.GetAllUsers().FirstOrDefault();
                if (user != null)
                    user.ShouldNotBeNull();

            }

            var domainServiceRegistered = LocalIocManager.IsRegistered(typeof(IUserDomainService));
            if (domainServiceRegistered)
            {
                var userDomainService = LocalIocManager.IocContainer.Resolve<IUserDomainService>();
            }

        }
    }
}
