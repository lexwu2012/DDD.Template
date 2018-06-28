using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Domain.Auditing;
using DDD.Domain.BaseEntities;
using DDD.Domain.Common.BaseEntities;
using DDD.Domain.Entities;

namespace DDD.Domain.Core.Model
{
    public class Reader : FullAuditedEntity, IAggregateRoot
    {
        public void Borrow(Book book)
        {
            if(book.RegistrationStatus == RegistrationStatus.Lend)
                throw new Exception();
        }
    }
}
