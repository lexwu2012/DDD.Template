using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace ThemePark.Infrastructure.Services
{
    public class SessionContextInitializer : ICallContextInitializer
    {
        /// <summary>实现它来参与初始化操作线程。</summary>
        /// <returns>作为 <see cref="M:System.ServiceModel.Dispatcher.ICallContextInitializer.AfterInvoke(System.Object)" /> 方法的参数传回的关联对象。</returns>
        /// <param name="instanceContext">操作的服务实例。</param>
        /// <param name="channel">客户端通道。</param>
        /// <param name="message">传入消息。</param>
        public object BeforeInvoke(InstanceContext instanceContext, IClientChannel channel, Message message)
        {
            if (!message.Headers.Any(o => o.Name.Equals(SessionContext.ContextName) && o.Namespace.Equals(SessionContext.ContextNs)))
            {
                return new object();
            }

            var data = message.Headers.GetHeader<SessionContext>(SessionContext.ContextName, SessionContext.ContextNs);
            CallContext.LogicalSetData(SessionKey.UserId, data.UserId);
            CallContext.LogicalSetData(SessionKey.TenantId, data.TenantId);
            CallContext.LogicalSetData(SessionKey.ImpersonatorUserId, data.ImpersonatorUserId);
            CallContext.LogicalSetData(SessionKey.ImpersonatorTenantId, data.ImpersonatorTenantId);

            return data;
        }

        /// <summary>实现它来参与清理调用该操作的线程。</summary>
        /// <param name="correlationState">从 <see cref="M:System.ServiceModel.Dispatcher.ICallContextInitializer.BeforeInvoke(System.ServiceModel.InstanceContext,System.ServiceModel.IClientChannel,System.ServiceModel.Channels.Message)" /> 方法返回的关联对象。</param>
        public void AfterInvoke(object correlationState)
        {
            var data = correlationState as SessionContext;
            if (data != null)
            {
                
            }

            CallContext.FreeNamedDataSlot(SessionKey.UserId);
            CallContext.FreeNamedDataSlot(SessionKey.TenantId);
            CallContext.FreeNamedDataSlot(SessionKey.ImpersonatorUserId);
            CallContext.FreeNamedDataSlot(SessionKey.ImpersonatorTenantId);
        }
    }
}
