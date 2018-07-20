using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Domain.Core.DbContextRelate;
using DDD.Domain.Core.Model;
using DDD.Domain.Core.Model.ValueObj;

namespace DDD.Domain.Core.Migrations.SeedData
{
    public class DefaultCreator
    {
        private readonly DDDDbContext _dbContext;

        public DefaultCreator(DDDDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public void Create()
        {
            AddUserIfNotExists("admin");
        }


        private void AddUserIfNotExists(string name)
        {
            //if (_dbContext.Users.Any(s => s.Name == name))
            //{
            //    return;
            //}

            //_dbContext.Users.Add(new User
            //{
            //    Name = name,
            //    Address = new Address
            //    {
            //        Street = "OverSky"
            //    }
            //});
            //_dbContext.SaveChanges();
        }
    }
}
