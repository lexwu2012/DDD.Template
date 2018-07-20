using System.Data.Common;
using DDD.Application.Service.User.Interfaces;
using DDD.Test;
using Xunit;
using System.Data.Entity;
using System.Linq.Dynamic;
using Castle.MicroKernel.Registration;
using DDD.Domain.Core.Model;
using DDD.Domain.Core.Model.Repositories.Interfaces;
using DDD.Domain.Core.Model.ValueObj;
using Effort.Provider;
using Shouldly;
using DDD.Infrastructure.Ioc.Dependency;
using DDD.Domain.Core.Repositories;
using System.Linq;
using DDD.Domain.Core.Model.Repositories.Dto;
using DDD.Infrastructure.Domain.Uow;

namespace DDD.SqlServer.Test.Uow
{
    public class UowTests : IntegratedTestBase
    {
        public UowTests()
        {
            EffortProviderConfiguration.RegisterProvider();

            LocalIocManager.IocContainer.Register(
                Component.For<DbConnection>()
                    .UsingFactoryMethod(Effort.DbConnectionFactory.CreateTransient)
                    .LifestyleSingleton()
                );

            //注册dbcontext中的实体泛型仓储
            using (var repositoryRegistrar = LocalIocManager.ResolveAsDisposable<EfGenericRepositoryRegistrar>())
            {
                repositoryRegistrar.Object.RegisterForDbContext(typeof(SampleDbContext), LocalIocManager, EfAutoRepositoryTypes.Default);
            }

            CreateInitialData();
        }

        [Fact]
        public void Should_Rollback_If_Uow_Is_Not_Completed()
        {
            var userAppService = LocalIocManager.IocContainer.Resolve<IUserAppService>();

            int userCount = 0;

            UsingDbContext(
               context =>
               {
                   userCount = context.Users.Count();
               });

            //CreatePersonAsync will use same UOW.
            using (var uow = LocalIocManager.Resolve<IUnitOfWorkManager>().Begin())
            {
                var result = userAppService.AddUser(new AddUserInput { Name = "john" });
                //await uow.CompleteAsync(); //It's intentionally removed from code to see roll-back
            }

            //john will not be added since uow is not completed (so, rolled back)
            UsingDbContext(context =>
            {
                context.Users.FirstOrDefault(p => p.Name == "john").ShouldBeNull();
                context.Users.Count().ShouldBe(userCount);
            });
        }

        [Fact]
        public void Should_Complete_If_Uow_Is_Completed()
        {
            var userAppService = LocalIocManager.IocContainer.Resolve<IUserAppService>();

            int userCount = 0;

            UsingDbContext(
               context =>
               {
                   userCount = context.Users.Count();
               });

            //CreatePersonAsync will use same UOW.
            using (var uow = LocalIocManager.Resolve<IUnitOfWorkManager>().Begin())
            {
                var result = userAppService.AddUser(new AddUserInput { Name = "john", Address = new Address { Street = "NewTown" } });
                result.Data.ShouldBeGreaterThanOrEqualTo(0);
                //用来确认事务是否提交
                uow.Complete();
            }

            //john will not be added since uow is not completed (so, rolled back)
            UsingDbContext(context =>
            {
                context.Users.FirstOrDefault(p => p.Name == "john").ShouldNotBeNull();
                context.Users.Count().ShouldBe(userCount + 1);
            });
        }

        protected void CreateInitialData()
        {
            UsingDbContext(context =>
            {
                context.Users.Add(
                        new User
                        {
                            Name = "lex1",
                            Address = new Address
                            {
                                Street = "nest"
                            }
                        });
            });
        }
    }
}
