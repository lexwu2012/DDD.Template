using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using DDD.Infrastructure.Web.Threading;

namespace DDD.Infrastructure.Domain.Uow
{
    /// <summary>
    /// uow拦截器
    /// </summary>
    public class UnitOfWorkInterceptor: IInterceptor
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IUnitOfWorkDefaultOptions _unitOfWorkOptions;

        public UnitOfWorkInterceptor(IUnitOfWorkDefaultOptions unitOfWorkOptions, IUnitOfWorkManager unitOfWorkManager)
        {
            _unitOfWorkOptions = unitOfWorkOptions;
            _unitOfWorkManager = unitOfWorkManager;
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


            PerformUow(invocation, unitOfWorkAttr.CreateOptions());
            //var options = unitOfWorkAttr.CreateOptions();
            //using (var uow = _unitOfWorkManager.Begin(options))
            //{
            //    invocation.Proceed();
            //    uow.Complete();
            //}

            //using (var ts = new TransactionScope())//创建一个事务范围对象
            //{
            //    invocation.Proceed();//执行被拦截的方法
            //    ts.Complete();//事务完成
            //}
        }

        private void PerformUow(IInvocation invocation, UnitOfWorkOptions options)
        {
            if (invocation.Method.IsAsync())
            {
                PerformAsyncUow(invocation, options);
            }
            else
            {
                PerformSyncUow(invocation, options);
            }
        }

        /// <summary>
        /// 同步方法
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="options"></param>
        private void PerformSyncUow(IInvocation invocation, UnitOfWorkOptions options)
        {
            using (var uow = _unitOfWorkManager.Begin(options))
            {
                invocation.Proceed();
                uow.Complete();
            }
        }

        /// <summary>
        /// 异步方法，使用异步UOW
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="options"></param>
        private void PerformAsyncUow(IInvocation invocation, UnitOfWorkOptions options)
        {
            var uow = _unitOfWorkManager.Begin(options);

            try
            {
                invocation.Proceed();
            }
            catch
            {
                uow.Dispose();
                throw;
            }

            if (invocation.Method.ReturnType == typeof(Task))
            {
                invocation.ReturnValue = InternalAsyncHelper.AwaitTaskWithPostActionAndFinally(
                    (Task)invocation.ReturnValue,
                    async () => await uow.CompleteAsync(),
                    exception => uow.Dispose()
                );
            }
            else
            {
                invocation.ReturnValue = InternalAsyncHelper.CallAwaitTaskWithPostActionAndFinallyAndGetResult(
                    invocation.Method.ReturnType.GenericTypeArguments[0],
                    invocation.ReturnValue,
                    async () => await uow.CompleteAsync(),
                    exception => uow.Dispose()
                );
            }
        }
    }
}
