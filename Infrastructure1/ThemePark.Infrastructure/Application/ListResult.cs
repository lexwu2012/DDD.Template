using System.Collections.Generic;

namespace ThemePark.Infrastructure.Application
{
    /// <summary>
    /// 返回结果集
    /// </summary>
    public class ListResult<TData> : Result<IList<TData>>
    {
        /// <summary>
        /// 返回结果集
        /// </summary>
        public ListResult()
        {
            Data = new List<TData>();
        }

        /// <summary>
        /// 返回结果集
        /// </summary>
        public ListResult(IList<TData> data)
            : base(data)
        {
        }

        /// <summary>
        /// 返回结果集
        /// </summary>
        /// <param name="code">状态码</param>
        /// <param name="message">提示信息</param>
        public ListResult(ResultCode code, string message = null)
            : base(code, message)
        {
            Data = new List<TData>();
        }
    }
}