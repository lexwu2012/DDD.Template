using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Infrastructure.Domain.Auditing;

namespace DDD.Domain.Core.Model
{
    /// <summary>
    /// 博客的回复
    /// </summary>
    public class Reply : FullAuditedEntity
    {
        /// <summary>
        /// 哪个人的回复
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// 回复属于哪个博客
        /// </summary>
        public virtual Post Post { get; set; }
    }
}
