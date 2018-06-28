using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Castle.Core.Logging;

namespace MyWebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    //[Authorize]
    public abstract class ApiControllerBase : ApiController
    {
        public ILogger Logger { protected get; set; } = NullLogger.Instance;
    }
}