using System;

namespace ThemePark.Infrastructure.Application
{
    /// <summary>
    /// Auto Map 属性映射
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MapFromAttribute : Attribute
    {
        /// <summary>
        /// 对应属性路径
        /// </summary>
        public string[] PropertyPath { get; set; }

        /// <summary>
        /// 属性映射
        /// </summary>
        public MapFromAttribute(params string[] propertyPath)
        {
            PropertyPath = propertyPath;
        }
    }
}