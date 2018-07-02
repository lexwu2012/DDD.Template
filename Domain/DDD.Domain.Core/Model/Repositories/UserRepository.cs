using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DDD.Domain.Core.Model.Repositories.Interfaces;
using DDD.Domain.Core.Model.ValueObj;
using DDD.Domain.Core.Repositories;
using DDD.Infrastructure.Web.Application;

namespace DDD.Domain.Core.Model.Repositories
{
    public class UserRepository: DDDRepositoryWithDbContext<User,long>, IUserRepository
    {
        public IQueryable<User> GetAllUsers()
        {
            return GetAll();
        }

        public Address GetAddress(int userId)
        {
            return GetAll().First(m => m.Id == userId).Address;
        }

        public User GetSepcifyUser(Expression<Func<User,bool>> expression)
        {
            return FirstOrDefault(expression);
        }

        public Result AddUser(User user)
        {
            if(null == user)
                return Result.FromError("用户不能为空");
            
            if(this.AsNoTracking().Any(m => m.Name == user.Name))
                return Result.FromError("已存在相同用户");

            this.Insert(user);

            //todo: 持久化？？

            return Result.FromData(user);
        }
    }
}
