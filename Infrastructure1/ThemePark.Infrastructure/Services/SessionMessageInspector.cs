using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Abp.Runtime.Session;

namespace ThemePark.Infrastructure.Services
{
    public class SessionMessageInspector : IClientMessageInspector
    {
        public IAbpSession AbpSession { get; set; }

        /// <summary>在将请求消息发送到服务之前，启用消息的检查或修改。</summary>
        /// <returns>作为 <see cref="M:System.ServiceModel.Dispatcher.IClientMessageInspector.AfterReceiveReply(System.ServiceModel.Channels.Message@,System.Object)" /> 方法的 <paramref name="correlationState " />参数返回的对象。如果不使用关联状态，则为 null。最佳做法是将它设置为 <see cref="T:System.Guid" />，以确保没有两个相同的 <paramref name="correlationState" /> 对象。</returns>
        /// <param name="request">要发送给服务的消息。</param>
        /// <param name="channel">WCF 客户端对象通道。</param>
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var data = new SessionContext(AbpSession);
            var header = new MessageHeader<SessionContext>(data);
            request.Headers.Add(header.GetUntypedHeader(SessionContext.ContextName, SessionContext.ContextNs));

            return null;
        }

        /// <summary>在收到回复消息之后将它传递回客户端应用程序之前，启用消息的检查或修改。</summary>
        /// <param name="reply">要转换为类型并交回给客户端应用程序的消息。</param>
        /// <param name="correlationState">关联状态数据。</param>
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            return;
        }
    }
}
