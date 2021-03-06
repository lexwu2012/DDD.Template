﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDD.Infrastructure.Common;
using DDD.Infrastructure.Domain.Events.Factories;
using DDD.Infrastructure.Domain.Events.Handlers;

namespace DDD.Infrastructure.Domain.Events
{
    public sealed class NullEventBus : IEventBus
    {
        
        public static NullEventBus Instance { get { return SingletonInstance; } }

        private static readonly NullEventBus SingletonInstance = new NullEventBus();

        private NullEventBus()
        {
        }

        /// <inheritdoc/>
        public IDisposable Register<TEventData>(Action<TEventData> action) where TEventData : IEventData
        {
            return NullDisposable.Instance;
        }

        /// <inheritdoc/>
        public IDisposable Register<TEventData>(IEventHandlerWithTEventData<TEventData> handler) where TEventData : IEventData
        {
            return NullDisposable.Instance;
        }


        /// <inheritdoc/>
        public IDisposable Register<TEventData, THandler>()
            where TEventData : IEventData
            where THandler : IEventHandler, new()
        {
            return NullDisposable.Instance;
        }

        /// <inheritdoc/>
        public IDisposable Register(Type eventType, IEventHandler handler)
        {
            return NullDisposable.Instance;
        }

        /// <inheritdoc/>
        public IDisposable Register<TEventData>(IEventHandlerFactory handlerFactory) where TEventData : IEventData
        {
            return NullDisposable.Instance;
        }

        /// <inheritdoc/>
        public IDisposable Register(Type eventType, IEventHandlerFactory handlerFactory)
        {
            return NullDisposable.Instance;
        }

        /// <inheritdoc/>
        public void Unregister<TEventData>(Action<TEventData> action) where TEventData : IEventData
        {
        }

        /// <inheritdoc/>
        public void Unregister<TEventData>(IEventHandlerWithTEventData<TEventData> handler) where TEventData : IEventData
        {
        }

        /// <inheritdoc/>
        public void Unregister(Type eventType, IEventHandler handler)
        {
        }

        /// <inheritdoc/>
        public void Unregister<TEventData>(IEventHandlerFactory factory) where TEventData : IEventData
        {
        }

        /// <inheritdoc/>
        public void Unregister(Type eventType, IEventHandlerFactory factory)
        {
        }

        /// <inheritdoc/>
        public void UnregisterAll<TEventData>() where TEventData : IEventData
        {
        }

        /// <inheritdoc/>
        public void UnregisterAll(Type eventType)
        {
        }

        /// <inheritdoc/>
        public void Trigger<TEventData>(TEventData eventData) where TEventData : IEventData
        {
        }

        /// <inheritdoc/>
        public void Trigger<TEventData>(object eventSource, TEventData eventData) where TEventData : IEventData
        {
        }

        /// <inheritdoc/>
        public void Trigger(Type eventType, IEventData eventData)
        {
        }

        /// <inheritdoc/>
        public void Trigger(Type eventType, object eventSource, IEventData eventData)
        {
        }
    }
}
