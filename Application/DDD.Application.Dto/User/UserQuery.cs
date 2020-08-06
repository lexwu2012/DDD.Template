using DDD.Domain.Core.Model;
using DDD.Infrastructure.Web.CustomAttributes;
using DDD.Infrastructure.Web.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Application.Dto.User
{
    public class UserQuery : Query<Domain.Core.Model.User>
    {
        /// <summary>
        /// 名字
        /// </summary>
        [QueryMode(QueryCompare.Like,nameof(Domain.Core.Model.User.Name))]
        public string Name { get; set; }

        /// <summary>
        /// 身份证
        /// </summary>
        [QueryMode(QueryCompare.Like, nameof(Domain.Core.Model.User.Identity))]
        public long Identity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [QueryMode(QueryCompare.Equal, nameof(Domain.Core.Model.User.Gender))]
        public Gender Gender { get; set; }
    }
}
