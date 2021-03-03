using System.Transactions;
using DDD.Infrastructure.Ioc.Dependency;

namespace DDD.Infrastructure.Domain.Uow
{
    /// <summary>
    /// facade模式，对外提供uow方法
    /// </summary>
    public class UnitOfWorkManager : IUnitOfWorkManager, ITransientDependency
    {
        private readonly IIocResolver _iocResolver;
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;
        private readonly IUnitOfWorkDefaultOptions _defaultOptions;

        public IActiveUnitOfWork Current  => _currentUnitOfWorkProvider.Current;

        public UnitOfWorkManager(
           IIocResolver iocResolver,
           ICurrentUnitOfWorkProvider currentUnitOfWorkProvider,
           IUnitOfWorkDefaultOptions defaultOptions)
        {
            _iocResolver = iocResolver;
            _currentUnitOfWorkProvider = currentUnitOfWorkProvider;
            _defaultOptions = defaultOptions;
        }


        public IUnitOfWorkCompleteHandle Begin()
        {
            return Begin(new UnitOfWorkOptions());
        }

        public IUnitOfWorkCompleteHandle Begin(TransactionScopeOption scope)
        {
            return Begin(new UnitOfWorkOptions { Scope = scope });
        }

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public IUnitOfWorkCompleteHandle Begin(UnitOfWorkOptions options)
        {
            options.FillDefaultsForNonProvidedOptions(_defaultOptions);

            //检查是否已经存开启了第一个工作单元，有的话获取当前的第一个工作单元
            var outerUow = _currentUnitOfWorkProvider.Current;

            //如果当前已经存在第一个事务，则返回InnerUnitOfWorkCompleteHandle
            if (options.Scope == TransactionScopeOption.Required && outerUow != null)
            {
                //这个对象的Complete方法不会做事务的提交，只是一个uow链式嵌套
                return new InnerUnitOfWorkCompleteHandle();
            }

            //解析第一个UnitOfWork
            var uow = _iocResolver.Resolve<IUnitOfWork>();

            //注册IUnitOfWork提交后事务后的提交委托（把当前的uow设置为null），在OnDisposed方法中执行这个Completed委托
            uow.Completed += (sender, args) =>
            {
                _currentUnitOfWorkProvider.Current = null;
            };

            //注册IUnitOfWork提交后事务后的失败委托（把当前的uow设置为null）,在OnDisposed方法中执行这个Failed委托
            uow.Failed += (sender, args) =>
            {
                _currentUnitOfWorkProvider.Current = null;
            };

            //注册IUnitOfWork提交后事务后的释放委托（把当前的uow设置为释放）,在OnDisposed方法中执行这个Disposed委托
            uow.Disposed += (sender, args) =>
            {
                _iocResolver.Release(uow);
            };

            //开启第一个真正的TransactionScope事务
            uow.Begin(options);

            //设置当前请求的uow不为空
            _currentUnitOfWorkProvider.Current = uow;

            return uow;
        }
    }
}
