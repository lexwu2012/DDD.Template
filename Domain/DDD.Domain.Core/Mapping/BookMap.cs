using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Domain.Core.Model;

namespace DDD.Domain.Core.Mapping
{
    public class BookMap : BaseMap<Book, string>
    {
        public BookMap() : base(nameof(Book))
        {
            Property(t => t.Name).HasColumnName("Name").IsRequired();

            //HasRequired(m => m.Address).WithMany().WillCascadeOnDelete(false);
        }
    }
}
