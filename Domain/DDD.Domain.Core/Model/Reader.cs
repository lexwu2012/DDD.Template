using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Infrastructure.Domain.Auditing;
using DDD.Infrastructure.Domain.BaseEntities;

namespace DDD.Domain.Core.Model
{
    public class Reader : FullAuditedEntity, IAggregateRoot
    {
        public string Remark { get; set; }

        public void Borrow(Book book)
        {
            if(book.RegistrationStatus == RegistrationStatus.Lend)
                throw new Exception();
        }
    }
}
