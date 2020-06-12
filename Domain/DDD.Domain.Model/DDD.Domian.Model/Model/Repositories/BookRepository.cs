using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Domain.Core.DbContextRelate;
using DDD.Domain.Core.Model.Repositories.Interfaces;
using DDD.Domain.Core.Repositories;

namespace DDD.Domain.Core.Model.Repositories
{
    public class BookRepository: DDDRepositoryWithDbContext<Book,string>, IBookRepository
    {
        public BookRepository(IDbContextProvider<DDDDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
    }
}
