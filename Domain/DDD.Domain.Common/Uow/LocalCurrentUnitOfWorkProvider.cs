using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using DDD.Infrastructure.Ioc.Dependency;
using System.Threading;

namespace DDD.Domain.Common.Uow
{
    public class LocalCurrentUnitOfWorkProvider : ICurrentUnitOfWorkProvider, ITransientDependency
    {
        [DoNotWire]
        public IUnitOfWork Current
        {
            get { return GetCurrentUow(); }
            set { SetCurrentUow(value); }
        }

        private static readonly AsyncLocal<LocalUowWrapper> AsyncLocalUow = new AsyncLocal<LocalUowWrapper>();

        private static IUnitOfWork GetCurrentUow()
        {
            var uow = AsyncLocalUow.Value?.UnitOfWork;
            if (uow == null)
            {
                return null;
            }

            if (uow.IsDisposed)
            {
                AsyncLocalUow.Value = null;
                return null;
            }

            return uow;

            ////获取当前工作单元key
            //var unitOfWorkKey = CallContext.LogicalGetData(ContextKey) as string;
            //if (unitOfWorkKey == null)
            //{
            //    return null;
            //}

            //IUnitOfWork unitOfWork;
            //if (!UnitOfWorkDictionary.TryGetValue(unitOfWorkKey, out unitOfWork))
            //{//如果根据key获取不到当前工作单元，那么就从当前线程集合（CallContext）中释放key
            //    CallContext.FreeNamedDataSlot(ContextKey);
            //    return null;
            //}

            //if (unitOfWork.IsDisposed)
            //{//如果当前工作单元已经dispose，那么就从工作单元集合中移除，并将key从当前线程集合（CallContext）中释放
            //    logger.Warn("There is a unitOfWorkKey in CallContext but the UOW was disposed!");
            //    UnitOfWorkDictionary.TryRemove(unitOfWorkKey, out unitOfWork);
            //    CallContext.FreeNamedDataSlot(ContextKey);
            //    return null;
            //}

            //return unitOfWork;
        }

        private static void SetCurrentUow(IUnitOfWork value)
        {
            lock (AsyncLocalUow)
            {
                if (value == null)
                {
                    if (AsyncLocalUow.Value == null)
                    {
                        return;
                    }

                    if (AsyncLocalUow.Value.UnitOfWork?.Outer == null)
                    {
                        AsyncLocalUow.Value.UnitOfWork = null;
                        AsyncLocalUow.Value = null;
                        return;
                    }

                    AsyncLocalUow.Value.UnitOfWork = AsyncLocalUow.Value.UnitOfWork.Outer;
                }
                else
                {
                    if (AsyncLocalUow.Value?.UnitOfWork == null)
                    {
                        if (AsyncLocalUow.Value != null)
                        {
                            AsyncLocalUow.Value.UnitOfWork = value;
                        }

                        AsyncLocalUow.Value = new LocalUowWrapper(value);
                        return;
                    }

                    value.Outer = AsyncLocalUow.Value.UnitOfWork;
                    AsyncLocalUow.Value.UnitOfWork = value;
                }
            }
        }

        private class LocalUowWrapper
        {
            public IUnitOfWork UnitOfWork { get; set; }

            public LocalUowWrapper(IUnitOfWork unitOfWork)
            {
                UnitOfWork = unitOfWork;
            }
        }
    }
}
