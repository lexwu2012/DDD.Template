using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DDD.Infrastructure.WebApi.Api
{
    internal static class MediaTypeHeaderValueExtensions
    {
        private static MethodInfo _internalIsSubsetof;

        static MediaTypeHeaderValueExtensions()
        {
            var internalType = Type.GetType("System.Net.Http.Formatting.MediaTypeHeaderValueExtensions, System.Net.Http.Formatting");
            _internalIsSubsetof = internalType.GetMethod("IsSubsetOf", new[] { typeof(MediaTypeHeaderValue), typeof(MediaTypeHeaderValue) });
        }

        public static bool IsSubsetOf(this MediaTypeHeaderValue mediaType1, MediaTypeHeaderValue mediaType2)
        {
            return (bool)_internalIsSubsetof.Invoke(null, new object[] { mediaType1, mediaType2 });
        }
    }
}
