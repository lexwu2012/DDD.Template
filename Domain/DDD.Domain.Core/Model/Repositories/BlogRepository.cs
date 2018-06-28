using System.Collections.Generic;
using System.Linq;
using DDD.Domain.Core.Model.Repositories.Interfaces;
using DDD.Domain.Core.Repositories;

namespace DDD.Domain.Core.Model.Repositories
{
    public class BlogRepository : DDDRepositoryWithDbContext<Blog>, IBlogRepository
    {
        private readonly IDictionary<int, Blog> _blogs;

        public BlogRepository(Dictionary<int, Blog> blogs)
        {
            _blogs = blogs;
            _blogs.Add(1, new Blog { Name = "blog1" });
            _blogs.Add(2, new Blog { Name = "blog2" });
            _blogs.Add(3, new Blog { Name = "blog3" });
        }


        public IQueryable<Blog> GetAllBolgByUserId(int userId)
        {
            return GetAll();
        }

        public ICollection<Post> GetAllPosts(int blogId)
        {
            return GetAll().First(m => m.Id == blogId).Posts;
        }

        ///// <summary>
        ///// 根据条件获取账户信息
        ///// </summary>
        //public async Task<TDto> GetAccountAsync<TDto>(IQuery<Post> query)
        //{
        //    return await FirstOrDefaultAsync<Post, TDto>(query);
        //}
    }
}
