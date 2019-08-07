using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using DDD.Infrastructure.Web.Threading;

namespace DDD.Infrastructure.Domain.Uow
{
    /// <summary>
    /// uow拦截器，描述被拦截后的方法动作
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

        /// <summary>
        /// 被拦截后的需要处理的内容
        /// </summary>
        /// <param name="invocation"></param>
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

            //检测该方法是否释放满足约定的拦截（ServiceBase,UnitOfWorkAttribute）
            var unitOfWorkAttr = _unitOfWorkOptions.GetUnitOfWorkAttributeOrNull(method);
            if (unitOfWorkAttr == null || unitOfWorkAttr.IsDisabled)
            {
                invocation.Proceed();
                return;
            }

            //区分同步和异步方法的事务开启和事务提交
            PerformUow(invocation, unitOfWorkAttr.CreateOptions());           
        }

        /// <summary>
        /// 根据同步异步开启对应的事务
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="options"></param>
        private void PerformUow(IInvocation invocation, UnitOfWorkOptions options)
        {
            if (invocation.Method.IsAsync())
            {
                //异步方法
                PerformAsyncUow(invocation, options);
            }
            else
            {
                //同步方法
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
                //直接提交
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
            //注意异步方法和同步方法开启的差异，没有使用using
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

            //根据返回结果进行事务提交
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
