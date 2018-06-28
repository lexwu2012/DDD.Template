﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Domain.Common.Repositories;
using DDD.Infrastructure.Web.Application;

namespace DDD.Domain.Core.Model.Repositories.Interfaces
{
    public interface IUserRepository : IRepositoryWithTEntityAndTPrimaryKey<User,long>
    {
        IQueryable<User> GetAllUsers();

        Result AddUser(User user);
    }
}
