using System;
using System.Linq.Expressions;

namespace ThemePark.Infrastructure.Application
{
    public class FilterParameter<TSource>
    {
        public Expression<Func<TSource, bool>> Predicate { get; set; }

        /// <summary>初始化 <see cref="T:ThemePark.Infrastructure.Application.FilterParameter`1" /> 类的新实例。</summary>
        public FilterParameter(Expression<Func<TSource, bool>> predicate)
        {
            Predicate = predicate;
        }
    }
}
