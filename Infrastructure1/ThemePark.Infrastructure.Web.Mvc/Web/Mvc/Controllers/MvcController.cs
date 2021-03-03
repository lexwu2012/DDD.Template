using System.Text;
using System.Web.Mvc;

namespace ThemePark.Infrastructure.Web.Mvc.Controllers
{
    /// <summary>
    /// MVC 控制器基类
    /// </summary>
    public class MvcController : Controller
    {
        /// <inheritdoc />
        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new MvcJsonResult(data)
            {
                CamelCase = true,
                ContentEncoding = contentEncoding,
                ContentType = contentType,
                JsonRequestBehavior = behavior
            };
        }
    }
}
