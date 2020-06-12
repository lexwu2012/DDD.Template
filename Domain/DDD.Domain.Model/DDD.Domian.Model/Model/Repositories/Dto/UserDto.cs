using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Domain.Core.Model.ValueObj;
using DDD.Infrastructure.AutoMapper.Attributes;

namespace DDD.Domain.Core.Model.Repositories.Dto
{
    [AutoMap(typeof(User))]
    public class UserDto
    {
        public string Name { get; set; }

        ///// <summary>
        ///// 身份证
        ///// </summary>
        //public long PIdentity { get; set; }

        public Address Address { get; set; }

        public string Remark { get; set; }
    }
}
