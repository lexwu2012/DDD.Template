using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.ModelBinding;
using DDD.Infrastructure.Common;
using DDD.Infrastructure.Common.Extensions;

namespace DDD.Infrastructure.WebApi.Api.Extension
{
    public static class ModelStateExtensions
    {
        /// <summary>
        /// 获取验证消息提示
        /// </summary>
        public static string GetValidationSummary(this ModelStateDictionary modelState, string separator = "\r\n")
        {
            if (modelState.IsValid) return null;

            var error = new StringBuilder();

            foreach (var item in modelState)
            {
                var state = item.Value;
                var message = state.Errors.FirstOrDefault(p => !p.ErrorMessage.IsNullOrEmpty())?.ErrorMessage;
                if (message.IsNullOrEmpty())
                {
                    message = state.Errors.FirstOrDefault(o => o.Exception != null)?.Exception.Message;
                }
                if (message.IsNullOrEmpty()) continue;

                if (error.Length > 0)
                {
                    error.Append(separator);
                }

                error.Append(item.Key + ":" + message);
            }

            return error.ToString();
        }
    }
}
