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
        /// <summary>
        /// 每个实体的主键（不包括值类型）
        /// </summary>
        TPrimaryKey Id { get; set; }


        bool IsTransient();
    }
}
