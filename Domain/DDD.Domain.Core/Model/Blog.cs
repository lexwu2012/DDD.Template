using System.Collections.Generic;
using DDD.Infrastructure.Domain.Auditing;
using DDD.Infrastructure.Domain.BaseEntities;

namespace DDD.Domain.Core.Model
{
    public class Blog : FullAuditedEntity, IAggregateRoot
    {
        public string Name { get; set; }

        public string Url { get; protected set; }

        public ICollection<Post> Posts { get; set; }

        public string Remark { get; set; }
        ///// <summary>
        ///// use IOC for injection
        ///// </summary>
        ///// <param name="blogRepository"></param>
        //public Blog(IBlogRepository blogRepository)
        //{
        //    _blogRepository = new BlogRepository();
        //}

        //public void Test()
        //{
        //    _blogRepository.GetAll();
        //}
    }
}
