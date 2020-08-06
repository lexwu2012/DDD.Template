using DDD.Domain.Core.Model;
using DDD.Infrastructure.AutoMapper.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Application.Dto.User
{
    [AutoMap(typeof(Domain.Core.Model.User))]
    public class UserDto
    {
        /// <summary>
        /// 名字
        /// </summary>      
        public string Name { get; set; }

        /// <summary>
        /// 身份证
        /// </summary>
        public long Identity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Gender Gender { get; set; }
    }
}
