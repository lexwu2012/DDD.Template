using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Domain.Core.Model;

namespace DDD.Domain.Core.Mapping
{
    public class UserMap : BaseMap<User, long>
    {
        public UserMap() : base(nameof(User))
        {
            Property(t => t.Name).HasColumnName("Name").IsRequired();

            //HasRequired(m => m.Address).WithMany().WillCascadeOnDelete(false);
        }
    }
}
