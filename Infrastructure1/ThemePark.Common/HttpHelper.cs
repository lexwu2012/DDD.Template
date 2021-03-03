using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ThemePark.Common
{
    /// <summary>
    /// http帮助类
    /// </summary>
    public static class HttpHelper
    {
        /// <summary>
        /// Post请求异步方法
        /// </summary>
        /// <param name="baseUri">域名地址</param>
        /// <param name="requestUri">请求接口</param>
        /// <param name="content">发送内容</param>
        /// <returns></returns>
        public static async Task<string> PostAsync(string baseUri, string requestUri, string content)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUri);
                //client.Timeout = TimeSpan.FromMinutes(30);
                //清除所有默认Content-Type
                client.DefaultRequestHeaders.Accept.Clear();
                //添加指定返回Content-Type
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PostAsync(requestUri, new StringContent(content, Encoding.UTF8, "application/json"));
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }
            return "";
        }

        /// <summary>
        /// Post请求异步方法
        /// </summary>
        /// <param name="baseUri">域名地址</param>
        /// <param name="requestUri">请求接口</param>
        /// <param name="content">发送内容</param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> PostRequestAsync(string baseUri, string requestUri, string content)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUri);
                //client.Timeout = TimeSpan.FromMinutes(30);
                //清除所有默认Content-Type
                client.DefaultRequestHeaders.Accept.Clear();
                //添加指定返回Content-Type
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                return await client.PostAsync(requestUri, new StringContent(content, Encoding.UTF8, "application/json"));
            }
        }

        /// <summary>
        /// Get请求异步方法
        /// </summary>
        /// <param name="baseUri">域名地址</param>
        /// <param name="requestUri">请求接口</param>
        /// <returns></returns>
        public static async Task<string> GetAsync(string baseUri, string requestUri)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUri);
                //client.Timeout = TimeSpan.FromMinutes(30);
                //清除所有默认Content-Type
                client.DefaultRequestHeaders.Accept.Clear();
                //添加指定返回Content-Type
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }

            return "";
        }

        /// <summary>
        /// Post请求同步方法
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="requestUri"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string Post(string baseUri, string requestUri, string content)
        {
            var task = PostAsync(baseUri, requestUri, content);
            try
            {
                task.Wait();
            }
            catch (AggregateException ex)
            {
                throw;
            }

            return task.Result;
        }

        /// <summary>
        /// Http协议Post方法
        /// </summary>
        /// <param name="url">目标地址</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static string HttpPost(string url, string param)
        {
            //System.Net.ServicePointManager.DefaultConnectionLimit = 20;
            byte[] bs = Encoding.UTF8.GetBytes(param);
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = bs.Length;
            WebResponse wr = null;

            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(bs, 0, bs.Length);
            }
            try
            {
                wr = req.GetResponse();
                StreamReader sr = new StreamReader(wr.GetResponseStream());
                return sr.ReadToEnd();
            }
            catch (Exception e)
            {
                req.Abort();
                wr?.Close();
                throw;
            }
        }
    }
}
