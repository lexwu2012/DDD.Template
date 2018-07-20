using System.Collections.Generic;
using DDD.Infrastructure.Domain.Repositories;

namespace DDD.Domain.Core.Model.Repositories.Interfaces
{
    public interface IBlogRepository : IRepositoryWithEntity<Blog>
    {
        ICollection<Post> GetAllPosts(int blogId);


        /// <summary>
        /// 根据条件获取账户信息
        /// </summary>
        //Task<TDto> GetAccountAsync<TDto>(IQuery<Post> query);
    }
}
