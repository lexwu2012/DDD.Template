using System.Data.Common;
using DDD.Application.Service.User.Interfaces;
using DDD.Test;
using Xunit;
using System.Data.Entity;
using System.Linq.Dynamic;
using Castle.MicroKernel.Registration;
using DDD.Application.Dto.User;
using DDD.Domain.Common.Uow;
using DDD.Domain.Core.Model;
using DDD.Domain.Core.Model.Repositories.Interfaces;
using DDD.Domain.Core.Model.ValueObj;
using DDD.Domain.Uow;
using Effort.Provider;
using Shouldly;

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

            CreateInitialData();
        }

        [Fact]
        public void Should_Rollback_If_Uow_Is_Not_Completed()
        {
            var userAppService = LocalIocManager.IocContainer.Resolve<IAddUserAppService>();

            int userCount = 0;

            UsingDbContext(
               context =>
               {
                   userCount = context.Users.Count();
               });

            //CreatePersonAsync will use same UOW.
            using (var uow = CreateUow(new UnitOfWorkOptions()))
            {
                var result = userAppService.AddUser(new AddUserInput { Name = "john" });
                //await uow.CompleteAsync(); //It's intentionally removed from code to see roll-back
            }

            //john will not be added since uow is not completed (so, rolled back)
            UsingDbContext(context =>
            {
                (context.Users.FirstOrDefaultAsync(p => p.Name == "john")).ShouldBe(null);
                context.Users.Count().ShouldBe(userCount);
            });
        }

        private IUnitOfWork CreateUow(UnitOfWorkOptions options)
        {
            var uow = LocalIocManager.Resolve<IUnitOfWork>();

            uow.Begin(options);

            return uow;
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
