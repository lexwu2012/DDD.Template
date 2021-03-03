using System;
using System.IO;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ThemePark.Infrastructure.Web.Mvc
{
    /// <inheritdoc />
    public class MvcJsonResult : JsonResult
    {
        /// <inheritdoc />
        public MvcJsonResult()
        {
            JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            CamelCase = true;
        }

        /// <inheritdoc />
        public MvcJsonResult(object data)
            :this()
        {
            Data = data;
        }

        /// <summary>
        /// 是否改为驼峰命名
        /// </summary>
        public bool CamelCase { get; set; }

        /// <summary>
        /// 是否缩进
        /// </summary>
        public bool Indented { get; set; }

        /// <summary>
        /// Json格式化配置
        /// </summary>
        public JsonSerializerSettings SerializerSettings { get; set; }

        /// <inheritdoc />
        public override void ExecuteResult(ControllerContext context)
        {
            var ignoreJsonRequestBehaviorDenyGet = false;
            if (context.HttpContext.Items.Contains("IgnoreJsonRequestBehaviorDenyGet"))
            {
                ignoreJsonRequestBehaviorDenyGet = string.Equals(context.HttpContext.Items["IgnoreJsonRequestBehaviorDenyGet"].ToString(), "true", StringComparison.OrdinalIgnoreCase);
            }

            if (!ignoreJsonRequestBehaviorDenyGet && JsonRequestBehavior == JsonRequestBehavior.DenyGet && string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("This request has been blocked because sensitive information could be disclosed to third party web sites when this is used in a GET request. To allow GET requests, set JsonRequestBehavior to AllowGet.");
            }

            var response = context.HttpContext.Response;

            response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";
            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }

            if (Data != null)
            {
                var serializer = JsonSerializer.CreateDefault(SerializerSettings);
                if (Indented)
                    serializer.Formatting = Formatting.Indented;

                if (CamelCase)
                {
                    serializer.ContractResolver = new CamelCasePropertyNamesContractResolver()
                    {
                        IgnoreSerializableAttribute = true,
                    };
                }

                using (var writer = new JsonTextWriter(new StreamWriter(response.OutputStream)))
                {
                    serializer.Serialize(writer, Data);
                }
            }
        }
    }
}
