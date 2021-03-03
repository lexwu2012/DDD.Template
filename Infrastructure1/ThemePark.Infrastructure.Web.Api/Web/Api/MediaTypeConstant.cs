using System.Net.Http.Headers;

namespace ThemePark.Infrastructure.Web.Api
{
    /// <summary>
    /// 常用 Content-Type
    /// </summary>
    public class MediaTypeConstant
    {
        /// <summary>
        /// application/xml
        /// </summary>
        public static readonly MediaTypeHeaderValue ApplicationXml = new MediaTypeHeaderValue("application/xml");

        /// <summary>
        /// text/xml
        /// </summary>
        public static readonly MediaTypeHeaderValue TextXml = new MediaTypeHeaderValue("text/xml");

        /// <summary>
        /// application/json
        /// </summary>
        public static readonly MediaTypeHeaderValue ApplicationJson = new MediaTypeHeaderValue("application/json");

        /// <summary>
        /// text/json
        /// </summary>
        public static readonly MediaTypeHeaderValue TextJson = new MediaTypeHeaderValue("text/json");

        /// <summary>
        /// application/bson
        /// </summary>
        public static readonly MediaTypeHeaderValue ApplicationBson = new MediaTypeHeaderValue("application/bson");

        /// <summary>
        /// application/x-www-form-urlencoded
        /// </summary>
        public static readonly MediaTypeHeaderValue ApplicationFormUrlEndoce =
            new MediaTypeHeaderValue("application/x-www-form-urlencoded");

        /// <summary>
        /// multipart/form-data
        /// </summary>
        public static readonly MediaTypeHeaderValue MultipartFormData =
            new MediaTypeHeaderValue("multipart/form-data");
    }
}