using System.Data.Common;
using System.Linq;
using Castle.MicroKernel.Registration;
using DDD.Domain.Core.Model;
using DDD.Domain.Core.Model.Repositories.Interfaces;
using DDD.Domain.Core.Model.ValueObj;
using Effort.Provider;
using Shouldly;
using Xunit;

namespace DDD.Test.EF
{
    public class SpecifiedObjCrudTest : IntegratedTestBase
    {
        private readonly IUserRepository _userRepository;
        

        public SpecifiedObjCrudTest()
        {
            EffortProviderConfiguration.RegisterProvider();

            LocalIocManager.IocContainer.Register(
                Component.For<DbConnection>()
                    .UsingFactoryMethod(Effort.DbConnectionFactory.CreateTransient)
                    .LifestyleSingleton()
                );


            _userRepository = LocalIocManager.IocContainer.Resolve<IUserRepository>();

            
            CreateInitialData();
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

        [Fact]
        public void Should_Delete_Entity_Not_In_Context()
        {
            var person = UsingDbContext(context => context.Users.Single(p => p.Name == "lex1"));
            _userRepository.Delete(person);
            UsingDbContext(context => context.Users.FirstOrDefault(p => p.Name == "lex1")).IsDeleted.ShouldBe(true);
        }
    }
}
