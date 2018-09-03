using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Domain.Core.Model;

namespace DDD.Domain.Core.Mapping
{
    public class ReaderMap : BaseMap<Reader, int>
    {
        public ReaderMap() : base(nameof(Reader))
        {
            Property(t => t.Name).HasColumnName("Name").IsRequired();

            //HasRequired(m => m.Address).WithMany().WillCascadeOnDelete(false);
        }
    }
}
