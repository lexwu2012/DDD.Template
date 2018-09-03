using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDD.Application.Service.User.Interfaces;
using DDD.Domain.Core.Model.Repositories.Interfaces;
using DDD.Infrastructure.AutoMapper.Extension;
using DDD.Infrastructure.Web.Application;
using DDD.Domain.Core.Model;
using DDD.Domain.Core.Model.Repositories.Dto;
using DDD.Domain.Core.Repositories;
using DDD.Infrastructure.Domain.Repositories;
using DDD.Infrastructure.Web.Query;

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

        public Result<UserDto> UpdateSpecifyUser(int id)
        {
            var user = _userRepository.GetAll().FirstOrDefault(m => m.Id == id);

            if (null == user)
                return Result.FromError<UserDto>("没有该用户");
            
            return Result.FromData<UserDto>(user.MapTo<UserDto>());
        }

        /// <summary>
        /// 获取单个用户信息
        /// </summary>
        public async Task<TDto> GetUserAsync<TDto>(IQuery<Domain.Core.Model.User> query)
        {
            return await _userRepository.AsNoTracking().FirstOrDefaultAsync<Domain.Core.Model.User, TDto>(query);            
        }

        /// <summary>
        /// 获取用户信息列表
        /// </summary>
        public async Task<IList<TDto>> GetAgencySaleTicketClassListAsync<TDto>(IQuery<Domain.Core.Model.User> query)
        {
            return await _userRepository.GetAll().ToListAsync<Domain.Core.Model.User, TDto>(query);
        }
    }
}
