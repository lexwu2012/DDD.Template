using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Domain.Common.Uow
{
    public interface IUnitOfWorkDefaultOptions
    {
        bool IsTransactional { get; set; }

        List<Func<Type, bool>> ConventionalUowSelectors { get; }
    }
}
