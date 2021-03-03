using System;
using System.Collections.Generic;

namespace ThemePark.Infrastructure.Application
{
    public class QueryParameter<TSource>
    {
        //sort
        public IList<SortField> SortFields { get; set; }

        //page
        public PageInfo PageInfo { get; set; }

        //filters
        public IList<FilterParameter<TSource>> FilterParameters { get; set; }

        //group s?
        //TODO: can group?
        //public GroupField<TSource, object> GroupField { get; set; }

        public QueryParameter()
        {
            SortFields = new List<SortField>();
            FilterParameters = new List<FilterParameter<TSource>>();
        }
    }

    public class QueryParameter<TSource, TResult> : QueryParameter<TSource>
    {
        public Func<TSource, TResult> Transform { get; set; }

        public QueryParameter(Func<TSource, TResult> transform)
        {
            if (transform == null)
            {
                throw new ArgumentException(nameof(transform));
            }

            Transform = transform;
        }
    }

    public class TestPara<T1>
    {

    }

    public class TestPara<T1, T2> : TestPara<T1>
    {
        
    }

    public class TestResult<T>
    {
        
    }

    
}
