using System.Collections.Generic;

namespace ThemePark.Infrastructure.Application
{
    public class QueryResult<T> : PageResult<T>
    {
        public int AllCount { get; set; }

        public QueryResult(IList<T> items, IPageInfo pageInfo, int total)
            : base(items, total, pageInfo)
        {
        }
    }
}
