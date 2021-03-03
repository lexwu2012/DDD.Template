using System;

namespace ThemePark.Infrastructure.Application
{
    /// <summary>
    /// 查询字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class QueryAttribute : Attribute
    {
        /// <summary>
        /// 比较方式
        /// </summary>
        public QueryCompare Compare { get; set; }

        /// <summary>
        /// 对应属性路径
        /// </summary>
        public string[] PropertyPath { get; set; }

        /// <summary>
        /// 查询字段
        /// </summary>
        public QueryAttribute(params string[] propertyPath)
        {
            PropertyPath = propertyPath;
        }
        
        /// <summary>
        /// 查询字段
        /// </summary>
        public QueryAttribute(QueryCompare compare, params string[] propertyPath)
        {
            PropertyPath = propertyPath;
            Compare = compare;
        }
    }
}