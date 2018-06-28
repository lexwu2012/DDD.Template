using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Domain.Entities;

namespace DDD.Domain.Auditing
{
    public interface IModificationAudited : IHasModificationTime
    {
        long? LastModifierUserId { get; set; }
    }
    
    public interface IModificationAudited<TUser> : IModificationAudited
        where TUser : IEntity<long>
    {
        TUser LastModifierUser { get; set; }
    }
}
