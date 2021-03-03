using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Async;
using Abp.Extensions;

namespace ThemePark.Infrastructure.Web.Mvc
{
    /// <summary>
    /// 
    /// </summary>
    public static class ActionDescriptorExtensions
    {
        /// <summary>
        /// 获取当着 Action 关联的 MethodInfo， 如果不存在则返回 null
        /// </summary>
        public static MethodInfo GetMethodInfoOrNull(this ActionDescriptor actionDescriptor)
        {
            if (actionDescriptor is ReflectedActionDescriptor)
            {
                return actionDescriptor.As<ReflectedActionDescriptor>().MethodInfo;
            }

            if (actionDescriptor is ReflectedAsyncActionDescriptor)
            {
                return actionDescriptor.As<ReflectedAsyncActionDescriptor>().MethodInfo;
            }

            if (actionDescriptor is TaskAsyncActionDescriptor)
            {
                return actionDescriptor.As<TaskAsyncActionDescriptor>().MethodInfo;
            }

            return null;
        }
    }
}