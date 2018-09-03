using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Castle.Core.Logging;
using DDD.Infrastructure.Common.Extensions;
using DDD.Infrastructure.Domain.Events.Factories;
using DDD.Infrastructure.Domain.Events.Handlers;
using DDD.Infrastructure.Web.Threading;

namespace DDD.Infrastructure.Domain.Events
{
    /// <summary>
    /// 事件总线
    /// </summary>
    public class EventBus : IEventBus
    {
        public static EventBus Default { get; } = new EventBus();

        public ILogger Logger { get; set; }

        private readonly ConcurrentDictionary<Type, List<IEventHandlerFactory>> _handlerFactories;

        /// <summary>
        /// cotr
        /// </summary>
        public EventBus()
        {
            _handlerFactories = new ConcurrentDictionary<Type, List<IEventHandlerFactory>>();
            Logger = NullLogger.Instance;
        }


        #region  Register

        public IDisposable Register<TEventData>(Action<TEventData> action) where TEventData : IEventData
        {
            return Register(typeof(TEventData), new ActionEventHandler<TEventData>(action));
        }


        public IDisposable Register(Type eventType, IEventHandler handler)
        {
            return Register(eventType, new SingleInstanceHandlerFactory(handler));
        }

        /// <inheritdoc/>
        public IDisposable Register(Type eventType, IEventHandlerFactory factory)
        {
            GetOrCreateHandlerFactories(eventType)
                .Locking(factories => factories.Add(factory));

            return new FactoryUnregistrar(this, eventType, factory);
        }

        /// <inheritdoc/>
        public IDisposable Register<TEventData, THandler>()
            where TEventData : IEventData
            where THandler : IEventHandler, new()
        {
            return Register(typeof(TEventData), new TransientEventHandlerFactory<THandler>());
        }
        
        public IDisposable Register<TEventData>(IEventHandlerFactory factory) where TEventData : IEventData
        {
            return Register(typeof(TEventData), factory);
        }
        #endregion

        #region Unregister
        
        public void Unregister<TEventData>(Action<TEventData> action) where TEventData : IEventData
        {
            GetOrCreateHandlerFactories(typeof(TEventData))
                .Locking(factories =>
                {
                    factories.RemoveAll(
                        factory =>
                        {
                            var singleInstanceFactory = factory as SingleInstanceHandlerFactory;
                            if (singleInstanceFactory == null)
                            {
                                return false;
                            }

                            var actionHandler = singleInstanceFactory.HandlerInstance as ActionEventHandler<TEventData>;
                            if (actionHandler == null)
                            {
                                return false;
                            }

                            return actionHandler.Action == action;
                        });
                });
        }


        public void Unregister<TEventData>(IEventHandlerWithTEventData<TEventData> handler) where TEventData : IEventData
        {
            throw new NotImplementedException();
        }


        /// <inheritdoc/>
        public void Unregister(Type eventType, IEventHandler handler)
        {
            GetOrCreateHandlerFactories(eventType)
                .Locking(factories =>
                {
                    factories.RemoveAll(
                        factory =>
                            factory is SingleInstanceHandlerFactory &&
                            (factory as SingleInstanceHandlerFactory).HandlerInstance == handler
                        );
                });
        }

        /// <inheritdoc/>
        public void Unregister<TEventData>(IEventHandlerFactory factory) where TEventData : IEventData
        {
            Unregister(typeof(TEventData), factory);
        }

        /// <inheritdoc/>
        public void Unregister(Type eventType, IEventHandlerFactory factory)
        {
            GetOrCreateHandlerFactories(eventType).Locking(factories => factories.Remove(factory));
        }

        /// <inheritdoc/>
        public void UnregisterAll<TEventData>() where TEventData : IEventData
        {
            UnregisterAll(typeof(TEventData));
        }

        /// <inheritdoc/>
        public void UnregisterAll(Type eventType)
        {
            GetOrCreateHandlerFactories(eventType).Locking(factories => factories.Clear());
        }
        #endregion

        #region Trigger

        /// <summary>
        /// 触发的eventData对应的IEventHandler实现（这在EventBusInstaller里面已经进行了eventData 对应的IEventHandler实现的注册）
        /// </summary>
        /// <typeparam name="TEventData"></typeparam>
        /// <param name="eventData"></param>
        public void Trigger<TEventData>(TEventData eventData) where TEventData : IEventData
        {
            Trigger((object)null, eventData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEventData"></typeparam>
        /// <param name="eventSource">IEventHandler的实现类</param>
        /// <param name="eventData">IEventHandler的参数</param>
        public void Trigger<TEventData>(object eventSource, TEventData eventData) where TEventData : IEventData
        {
            Trigger(typeof(TEventData), eventSource, eventData);
        }

        /// <inheritdoc/>
        public void Trigger(Type eventType, IEventData eventData)
        {
            Trigger(eventType, null, eventData);
        }
        
        /// <summary>
        /// 利用反射触发excute方法
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="eventSource"></param>
        /// <param name="eventData"></param>
        public void Trigger(Type eventType, object eventSource, IEventData eventData)
        {
            var exceptions = new List<Exception>();

            eventData.EventSource = eventSource;

            foreach (var handlerFactories in GetHandlerFactories(eventType))
            {
                foreach (var handlerFactory in handlerFactories.EventHandlerFactories)
                {
                    var handlerType = handlerFactory.GetHandlerType();

                    if (IsAsyncEventHandler(handlerType))
                    {
                        AsyncHelper.RunSync(() => TriggerAsyncHandlingException(handlerFactory, handlerFactories.EventType, eventData, exceptions));
                    }
                    else if (IsEventHandler(handlerType))
                    {
                        TriggerHandlingException(handlerFactory, handlerFactories.EventType, eventData, exceptions);
                    }
                    else
                    {
                        var message = $"Event handler to register for event type {eventType.Name} does not implement IEventHandler<{eventType.Name}> or IAsyncEventHandler<{eventType.Name}> interface!";
                        exceptions.Add(new Exception(message));
                    }
                }
            }

            if (exceptions.Any())
            {
                if (exceptions.Count == 1)
                {
                    exceptions[0].ReThrow();
                }

                throw new AggregateException("More than one error has occurred while triggering the event: " + eventType, exceptions);
            }
        }


        #endregion

        /// <summary>
        /// 同步方法，利用反射，执行EventHandler里面的HandleEvent方法
        /// </summary>
        /// <param name="handlerFactory"></param>
        /// <param name="eventType"></param>
        /// <param name="eventData"></param>
        /// <param name="exceptions"></param>
        /// <returns></returns>
        private void TriggerHandlingException(IEventHandlerFactory handlerFactory, Type eventType, IEventData eventData, List<Exception> exceptions)
        {
            var eventHandler = handlerFactory.GetHandler();
            try
            {
                if (eventHandler == null)
                {
                    throw new ArgumentNullException($"Registered event handler for event type {eventType.Name} is null!");
                }

                var handlerType = typeof(IEventHandlerWithTEventData<>).MakeGenericType(eventType);

                var method = handlerType.GetMethod(
                    "HandleEvent",
                    new[] { eventType }
                );

                method.Invoke(eventHandler, new object[] { eventData });
            }
            catch (TargetInvocationException ex)
            {
                exceptions.Add(ex.InnerException);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
            finally
            {
                handlerFactory.ReleaseHandler(eventHandler);
            }
        }

        /// <summary>
        /// 异步方法，利用反射，执行EventHandler里面的HandleEventAsync方法
        /// </summary>
        /// <param name="asyncHandlerFactory"></param>
        /// <param name="eventType"></param>
        /// <param name="eventData"></param>
        /// <param name="exceptions"></param>
        /// <returns></returns>
        private async Task TriggerAsyncHandlingException(IEventHandlerFactory asyncHandlerFactory, Type eventType, IEventData eventData, List<Exception> exceptions)
        {
            var asyncEventHandler = asyncHandlerFactory.GetHandler();

            try
            {
                if (asyncEventHandler == null)
                {
                    throw new ArgumentNullException($"Registered async event handler for event type {eventType.Name} is null!");
                }

                var asyncHandlerType = typeof(IAsyncEventHandler<>).MakeGenericType(eventType);

                var method = asyncHandlerType.GetMethod(
                    "HandleEventAsync",
                    new[] { eventType }
                );

                await (Task)method.Invoke(asyncEventHandler, new object[] { eventData });
            }
            catch (TargetInvocationException ex)
            {
                exceptions.Add(ex.InnerException);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
            finally
            {
                asyncHandlerFactory.ReleaseHandler(asyncEventHandler);
            }
        }

        private bool IsEventHandler(Type handlerType)
        {
            return handlerType.GetInterfaces()
                .Where(i => i.IsGenericType)
                .Any(i => i.GetGenericTypeDefinition() == typeof(IEventHandlerWithTEventData<>));
        }

        private bool IsAsyncEventHandler(Type handlerType)
        {
            return handlerType.GetInterfaces()
                .Where(i => i.IsGenericType)
                .Any(i => i.GetGenericTypeDefinition() == typeof(IAsyncEventHandler<>));
        }

        private IEnumerable<EventTypeWithEventHandlerFactories> GetHandlerFactories(Type eventType)
        {
            var handlerFactoryList = new List<EventTypeWithEventHandlerFactories>();

            foreach (var handlerFactory in _handlerFactories.Where(hf => ShouldTriggerEventForHandler(eventType, hf.Key)))
            {
                handlerFactoryList.Add(new EventTypeWithEventHandlerFactories(handlerFactory.Key, handlerFactory.Value));
            }

            return handlerFactoryList.ToArray();
        }

        private static bool ShouldTriggerEventForHandler(Type eventType, Type handlerType)
        {
            if (handlerType == eventType)
            {
                return true;
            }
            
            if (handlerType.IsAssignableFrom(eventType))
            {
                return true;
            }

            return false;
        }

        private List<IEventHandlerFactory> GetOrCreateHandlerFactories(Type eventType)
        {
            return _handlerFactories.GetOrAdd(eventType, (type) => new List<IEventHandlerFactory>());
        }

        private class EventTypeWithEventHandlerFactories
        {
            public Type EventType { get; }

            public List<IEventHandlerFactory> EventHandlerFactories { get; }

            public EventTypeWithEventHandlerFactories(Type eventType, List<IEventHandlerFactory> eventHandlerFactories)
            {
                EventType = eventType;
                EventHandlerFactories = eventHandlerFactories;
            }
        }

    }
}
