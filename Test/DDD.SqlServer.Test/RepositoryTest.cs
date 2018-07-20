using System.Linq;
using DDD.Domain.Core.Model;
using DDD.Domain.Core.Model.Repositories.Interfaces;
using DDD.Domain.Core.Model.ValueObj;
using DDD.Infrastructure.Ioc;
using Shouldly;
using Xunit;

namespace DDD.SqlServer.Test
{
    public class RepositoryTest //: TestBaseWithLocalIocManager
    {
        private readonly IUserRepository _userRepository;

        public readonly IIocManager LocalIocManager;

        public RepositoryTest()
        {
            //_userRepository = userRepository;
        }

        [Fact]
        public void QueryTest()
        {
            var users = _userRepository.GetAllUsers();
            users.Count().ShouldNotBeNull();
        }

        [Fact]
        public void InsertTest()
        {
            var registered = LocalIocManager.IsRegistered(typeof(IUserRepository));

            if (registered)
            {
                var userRepository = LocalIocManager.IocContainer.Resolve<IUserRepository>();

                var newUser = new User
                {
                    Address = new Address
                    {
                        Street = "金山街"
                    },
                    Name = "Winner"
                };

                var id = userRepository.InsertAndGetId(newUser);
                id.ShouldNotBeNull();

            }
        }
    }
}
