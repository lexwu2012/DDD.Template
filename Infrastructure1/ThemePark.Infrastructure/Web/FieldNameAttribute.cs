using System;

namespace ThemePark.Infrastructure.Web
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FieldNameAttribute : Attribute
    {
        public string FieldName { get; set; }
    }
}
