using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Domain.Entities
{

    public interface IEntity : IEntity<int>
    {

    }

    public interface IEntity<TPrimaryKey>
    {        
        TPrimaryKey Id { get; set; }

        bool IsTransient();
    }
}
