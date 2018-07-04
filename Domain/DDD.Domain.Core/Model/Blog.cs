﻿using System.Collections.Generic;
using DDD.Domain.Auditing;
using DDD.Domain.BaseEntities;

namespace DDD.Domain.Core.Model
{
    public class Blog : FullAuditedEntity, IAggregateRoot
    {
        public string Name { get; set; }

        public string Url { get; protected set; }

        public ICollection<Post> Posts { get; set; }

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