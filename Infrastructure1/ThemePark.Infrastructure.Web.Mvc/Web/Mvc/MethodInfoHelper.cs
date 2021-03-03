using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ThemePark.Infrastructure.Web.Mvc
{
    internal static class MethodInfoHelper
    {
        /// <summary>
        /// 判断当前 <see cref="MethodInfo"/> 是否为返回 <see cref="JsonResult"/>
        /// </summary>
        public static bool IsJsonResult(MethodInfo method)
        {
            return typeof(JsonResult).IsAssignableFrom(method.ReturnType) ||
                   typeof(Task<JsonResult>).IsAssignableFrom(method.ReturnType);
        }
    }
}
