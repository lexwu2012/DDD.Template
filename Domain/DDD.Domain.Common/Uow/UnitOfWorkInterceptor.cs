using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Castle.DynamicProxy;
using DDD.Domain.Uow;
using DDD.Infrastructure.Ioc.Dependency;

namespace DDD.Domain.Common.Uow
{
    public class UnitOfWorkInterceptor: IInterceptor
    {

        private readonly IIocResolver _iocResolver;
        private readonly IUnitOfWorkDefaultOptions _unitOfWorkOptions;

        public UnitOfWorkInterceptor(IUnitOfWorkDefaultOptions unitOfWorkOptions,
            IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
            _unitOfWorkOptions = unitOfWorkOptions;
        }

        public void Intercept(IInvocation invocation)
        {
            MethodInfo method;
            try
            {
                method = invocation.MethodInvocationTarget;
            }
            catch
            {
                method = invocation.GetConcreteMethod();
            }

            var unitOfWorkAttr = _unitOfWorkOptions.GetUnitOfWorkAttributeOrNull(method);
            if (unitOfWorkAttr == null || unitOfWorkAttr.IsDisabled)
            {
                invocation.Proceed();
                return;
            }

            var options = unitOfWorkAttr.CreateOptions();
            using (var uow = CreateUow(options))
            {
                invocation.Proceed();
                uow.Complete();
            }
            
            //using (var ts = new TransactionScope())//创建一个事务范围对象
            //{
            //    invocation.Proceed();//执行被拦截的方法
            //    ts.Complete();//事务完成
            //}
        }

        private IUnitOfWork CreateUow(UnitOfWorkOptions options)
        {
            var uow = _iocResolver.Resolve<IUnitOfWork>();

            uow.Begin(options);

            return uow;
        }
    }
}
