using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Domain.Common.Values;

namespace DDD.Domain.Core.Model.ValueObj
{
    public class Address : ValueObject<Address>
    {
        public string Street { get; set; }
    }
}
