using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.UI.WebControls;
using DDD.Infrastructure.Web.Application;
using MyWebApi.Controllers;

namespace DDD.WebApi.Controllers
{
    [RoutePrefix("api")]
    public class HomeController : ApiControllerBase
    {
        [HttpGet,Route("Index")]
        public Result Index()
        {
           return Result.Ok();
        }
    }
}
