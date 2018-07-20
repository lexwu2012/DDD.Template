using System.Linq;
using DDD.Application.Service.User.Interfaces;
using DDD.Domain.Core.Model.Repositories.Interfaces;
using DDD.Infrastructure.AutoMapper.Extension;
using DDD.Infrastructure.Web.Application;
using DDD.Domain.Core.Model;
using DDD.Domain.Core.Model.Repositories.Dto;
using DDD.Infrastructure.Domain.Repositories;

namespace DDD.Application.Service.User
{
    public class UserAppService : AppServiceBase, IUserAppService
    {
        //private readonly IUserRepository _userRepository;
        private readonly IRepositoryWithTEntityAndTPrimaryKey<Domain.Core.Model.User, long> _userRepository;

        public UserAppService(IRepositoryWithTEntityAndTPrimaryKey<Domain.Core.Model.User, long> userRepository)
        {
            _userRepository = userRepository;
        }

        public Result<int> AddUser(AddUserInput input)
        {
            var userCount = _userRepository.Count();

            var user = input.MapTo<Domain.Core.Model.User>();

            var result = _userRepository.Insert(user);

            if (result != null)
                return Result.FromData(userCount);

            return Result.FromError<int>("发生错误");
        }

        public Result<int> GetAllUsers()
        {
            var result = _userRepository.GetAll().Count();

            return Result.FromData(result);
        }
    }
}
