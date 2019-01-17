using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using DDD.Infrastructure.Ioc.Dependency;

namespace DDD.Infrastructure.Web.Mvc.Controller
{
    public class WindsorControllerFactory : DefaultControllerFactory
    {
        private readonly IIocResolver _iocManager;

        public WindsorControllerFactory(IIocResolver iocManager)
        {
            _iocManager = iocManager;
        }


        public override void ReleaseController(IController controller)
        {
            _iocManager.Release(controller);
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                return base.GetControllerInstance(requestContext, controllerType);
            }

            return _iocManager.Resolve<IController>(controllerType);
        }
    }
}
