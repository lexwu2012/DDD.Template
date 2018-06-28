using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace MyWebApi.Controllers
{
    [RoutePrefix("home")]
    public class HomeController : ApiControllerBase
    {
        [HttpGet, Route("index")]
        public HttpResponseMessage Index([FromBody] int a)
        {
            return new HttpResponseMessage
            {
                Content =
                     new StringContent("测试", Encoding.UTF8, "application/json")
            };
        }
    }
}
