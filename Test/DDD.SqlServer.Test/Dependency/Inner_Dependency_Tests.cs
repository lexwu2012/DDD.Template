using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using DDD.Application.Service.Wechat.Interfaces;
using DDD.Domain.Core;
using DDD.Domain.Core.DbContextRelate;
using DDD.Domain.Core.Repositories;
using DDD.Domain.Core.Uow;
using DDD.Domain.Service.Wechat.Dto;
using DDD.Infrastructure.Ioc;
using DDD.Infrastructure.Ioc.Dependency;
using Shouldly;
using Xunit;

namespace DDD.SqlServer.Test.Dependency
{
    public class Inner_Dependency_Tests : TestBaseWithLocalIocManager
    {
        public Inner_Dependency_Tests()
        {
            

            using (var repositoryRegistrar = LocalIocManager.ResolveAsDisposable<EfGenericRepositoryRegistrar>())
            {
                repositoryRegistrar.Object.RegisterForDbContext(typeof(DDDDbContext), LocalIocManager, EfAutoRepositoryTypes.Default);
            }

        }

        [Fact]
        public void Should_Resolve_Correct_Dependencies()
        {
            //using (var uow = LocalIocManager.ResolveAsDisposable<IUnitOfWork>())
            //{
            //    var appService = LocalIocManager.IocContainer.Resolve<IWechatRepayService>();
            //    var dto = new GetPayInfoDto
            //    {
            //        IdPerson = 22,
            //        SystemCode = "222",
            //        Channel = "333"
            //    };

            //    var info = appService.GetPayInfoAsync(dto);

            //    uow.Object.Complete();

            //    info.Success.ShouldBeTrue();


            //}

            var appService = LocalIocManager.IocContainer.Resolve<IWechatRepayService>();
            var dto = new GetPayInfoDto
            {
                IdPerson = 22,
                SystemCode = "222",
                Channel = "333"
            };

            var info = appService.GetPayInfoAsync(dto);
            info.Success.ShouldBeTrue();
        }
    }
}
