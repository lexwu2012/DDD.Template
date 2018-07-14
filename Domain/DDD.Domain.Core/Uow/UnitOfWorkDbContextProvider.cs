using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Domain.Common.Uow;

namespace DDD.Domain.Core.Uow
{
    public class UnitOfWorkDbContextProvider<TDbContext> : IDbContextProvider<TDbContext>
        where TDbContext : DbContext
    {

        //private readonly IUnitOfWork _currentUnitOfWork;

        //public UnitOfWorkDbContextProvider(IUnitOfWork currentUnitOfWork)
        //{
        //    _currentUnitOfWork = currentUnitOfWork;
        //}

        //public TDbContext GetDbContext()
        //{
        //    //return new DDDDbContext() as TDbContext;
        //    return _currentUnitOfWork.Current.GetDbContext<TDbContext>();
        //}

        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;

        /// <summary>
        /// Creates a new <see cref="UnitOfWorkDbContextProvider{TDbContext}"/>.
        /// </summary>
        /// <param name="currentUnitOfWorkProvider"></param>
        public UnitOfWorkDbContextProvider(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
        {
            _currentUnitOfWorkProvider = currentUnitOfWorkProvider;
        }


        public TDbContext GetDbContext()
        {
            if (null != _currentUnitOfWorkProvider.Current)
                return _currentUnitOfWorkProvider.Current.GetDbContext<TDbContext>();
            return null;
        }
    }
}
