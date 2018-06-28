﻿using DDD.Application.Dto.User;
using DDD.Domain.Core.Model.Repositories.Interfaces;
using DDD.Infrastructure.AutoMapper.Extension;
using DDD.Infrastructure.Web.Application;
using DDD.MyApplication.User.Interfaces;

namespace DDD.MyApplication.User
{
    public class AddUserAppService : AppServiceBase, IAddUserAppService
    {
        private readonly IUserRepository _userRepository;

        public AddUserAppService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Result AddUser(AddUserInput input)
        {
            var user = input.MapTo<Domain.Core.Model.User>();

            var result = _userRepository.AddUser(user);

            if (result.Success)
                return Result.Ok();

            return result;
        }
    }
}
