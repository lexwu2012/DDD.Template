using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using Castle.Core.Logging;
using DDD.Infrastructure.Common.Extensions;
using DDD.Infrastructure.Ioc.Dependency;
using DDD.Infrastructure.Web.Application;

namespace DDD.Infrastructure.WebApi.Api.Log
{
    public class HttpMessageLogHandler: DelegatingHandler,ITransientDependency
    {
        private readonly ILoggerFactory _loggerFactory;
        private static readonly IEnumerable<MediaTypeHeaderValue> LogMediaTypes;

        static HttpMessageLogHandler()
        {
            var logMediaTypes = ConfigurationManager.AppSettings["LogHttpMessageTypes"];
            LogMediaTypes = logMediaTypes?
                                 .Split(new[] { ';', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                 .Select(p => new MediaTypeHeaderValue(p)).ToArray()
                             ?? new MediaTypeHeaderValue[0];
        }

        public HttpMessageLogHandler(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            string requestText = null;
            string responseText = null;

            if (LogMediaTypes.Any(p => p.IsSubsetOf(request.Content?.Headers.ContentType)))
            {
                requestText = await request.Content.ReadAsStringAsync();
                if (MediaTypeConstant.ApplicationFormUrlEndoce.IsSubsetOf(request.Content?.Headers.ContentType)
                    && !string.IsNullOrWhiteSpace(requestText))
                {
                    var unescapeText = string.Join("\r\n", requestText.Split('&').Select(Uri.UnescapeDataString));
                    requestText += $"\r\n转义参数：\r\n{unescapeText}";
                }
            }

            var watcher = Stopwatch.StartNew();

            var response = await base.SendAsync(request, cancellationToken);

            watcher.Stop();

            var actionDescriptor = request.Properties.GetOrDefault("MS_HttpActionDescriptor") as ReflectedHttpActionDescriptor;
            var requestContext = request.Properties.GetOrDefault("MS_RequestContext") as HttpRequestContext;
            var user = requestContext?.Principal as ClaimsPrincipal;
            var actionName = actionDescriptor != null
                ? $"{actionDescriptor.ControllerDescriptor.ControllerType.FullName}" +
                  $".{actionDescriptor.MethodInfo.Name}"
                : null;

            IResult result;
            // 返回内容如果是文件流，会影响到客户端接收，所以只读取对像流
            if (response.TryGetContentValue(out result) || response.Content is ObjectContent || response.Content is StringContent)
                responseText = await response.Content.ReadAsStringAsync();
            if (result == null)
                result = Result.Ok();

            var userId = user?.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;
            var userName = user?.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Name)?.Value;
            var logMsg = $"接口：{actionName}\r\n" +
                         $"用户信息：{userId} {userName}\r\n" +
                         $"请求接口：{request.Method} {request.RequestUri}\r\n" +
                         $"请求内容：{request.Content.Headers.ContentType}\r\n" +
                         $"{requestText}\r\n" +
                         $"返回结果：{response.StatusCode:D} {response.ReasonPhrase}\r\n" +
                         $"耗费：{watcher.ElapsedMilliseconds}毫秒\r\n"+
                         $"返回内容：{response.Content?.Headers.ContentType}\r\n" +
                         $"{responseText}\r\n";

            var logName = $"{nameof(HttpMessageLogHandler)}.{actionName}";

            var log = _loggerFactory.Create(logName);

            if (result.Code == ResultCode.Ok)
            {
                if (log.IsInfoEnabled)
                    log.Info(logMsg);
                else if (log.IsDebugEnabled)
                    log.Debug(logMsg);
            }
            else
            {
                log.Info(logMsg);
            }

            return response;
        }
    }
}
