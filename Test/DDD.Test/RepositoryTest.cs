using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Domain.Core.Model;
using DDD.Domain.Core.Model.Repositories.Interfaces;
using DDD.Domain.Core.Model.ValueObj;
using Shouldly;
using Xunit;

namespace DDD.Test
{
    public class RepositoryTest : TestBaseWithLocalIocManager
    {
        private readonly IUserRepository _userRepository;
        public RepositoryTest(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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
