using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Domain.Core.Model.ValueObj;
using DDD.Infrastructure.Domain.Auditing;
using DDD.Infrastructure.Domain.BaseEntities;

namespace DDD.Domain.Core.Model
{
    //[Table(nameof(User))]
    public class User : FullAuditedEntity<long>, IAggregateRoot<long>
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

        /// <summary>
        /// 地址
        /// </summary>
        public Address Address { get; set; }


        public string Remark { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum Gender
    {
        Male = 10,

        Female = 20
    }
}
