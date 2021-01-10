using DotNetty.Buffers;
using DotNetty.Codecs.Http;
using DotNetty.Common.Utilities;
using NettyFrame.Server.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NettyFrame.Server.CoreImpl.Http
{
    public class OldHttpHandler : IHttpHandler
    {
        public async Task<IFullHttpResponse> GetHttpResponse(IFullHttpRequest request)
        {
            if (!request.Result.IsSuccess)
            {
                return GetHttpResponse(HttpResponseStatus.BadRequest);
            }
            //return GetHttpResponse(HttpResponseStatus.OK, "Hello World!");
            return await GetFileResponseAsync(request);
        }

        #region 私有方法
        #region 文件相关
        /// <summary>
        /// 获得文件Byte数组
        /// </summary>
        /// <param name="filePath"></param>
        private async Task<byte[]> GetFileBytesAsync(string filePath)
        {
            if (!File.Exists(filePath)) throw new Exception("文件不存在");
            byte[] result = await File.ReadAllBytesAsync(filePath);
            return result;
        }
        /// <summary>
        /// 获得文件返回
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<IFullHttpResponse> GetFileResponseAsync(IFullHttpRequest request)
        {
            string url = request.Uri == "/" ? "/Index.html" : request.Uri;
            string filePath = $"{AppDomain.CurrentDomain.BaseDirectory}HtmlPages{url}";
            string extension = Path.GetExtension(filePath);
            if (string.IsNullOrEmpty(extension)) return GetHttpResponse(HttpResponseStatus.NotFound);
            if (!File.Exists(filePath)) return GetHttpResponse(HttpResponseStatus.NotFound);
            byte[] body = await GetFileBytesAsync(filePath);
            string contentType = MIMEManager.GetContentType(extension);
            Dictionary<AsciiString, object> headers = GetDefaultHeaders(contentType);
            return GetHttpResponse(HttpResponseStatus.OK, body, headers);
        }
        #endregion

        private IFullHttpResponse GetHttpResponse(HttpResponseStatus status)
        {
            Dictionary<AsciiString, object> headers = GetDefaultHeaders();
            return GetHttpResponse(status, headers);
        }
        private IFullHttpResponse GetHttpResponse(HttpResponseStatus status, string body)
        {
            Dictionary<AsciiString, object> headers = GetDefaultHeaders("text/plain;charset-UTF-8");
            return GetHttpResponse(status, body, headers);

        }
        private IFullHttpResponse GetHttpResponse(HttpResponseStatus status, byte[] body)
        {
            Dictionary<AsciiString, object> headers = GetDefaultHeaders("application/octet-stream");
            return GetHttpResponse(status, body, headers);
        }
        private IFullHttpResponse GetHttpResponse(HttpResponseStatus status, Dictionary<AsciiString, object> headers)
        {
            return GetHttpResponse(status, string.Empty, headers);
        }
        private IFullHttpResponse GetHttpResponse(HttpResponseStatus status, string body, Dictionary<AsciiString, object> headers)
        {
            byte[] bodyData = string.IsNullOrEmpty(body) ? new byte[0] : Encoding.UTF8.GetBytes(body);
            return GetHttpResponse(status, bodyData, headers);
        }
        /// <summary>
        /// 基础返回
        /// </summary>
        /// <param name="status">状态吗</param>
        /// <param name="body"></param>
        /// <param name="headers"></param>
        private IFullHttpResponse GetHttpResponse(HttpResponseStatus status, byte[] body, Dictionary<AsciiString, object> headers)
        {
            DefaultFullHttpResponse result;
            if (body != null && body.Length > 0)
            {
                IByteBuffer bodyBuffer = Unpooled.WrappedBuffer(body);
                result = new DefaultFullHttpResponse(HttpVersion.Http11, status, bodyBuffer);
            }
            else
            {
                result = new DefaultFullHttpResponse(HttpVersion.Http11, status);
            }
            foreach ((AsciiString key, object value) in headers)
            {
                result.Headers.Set(key, value);
            }
            return result;
        }
        /// <summary>
        /// 获得默认的HttpHeaders
        /// </summary>
        private Dictionary<AsciiString, object> GetDefaultHeaders(string contentType = null)
        {
            var result = new Dictionary<AsciiString, object>
            {
                {HttpHeaderNames.Date, DateTime.Now },
                {HttpHeaderNames.Server,"MateralDotNettyServer" },
                {HttpHeaderNames.AcceptLanguage,"zh-CN,zh;q=0.9" }
            };
            if (!string.IsNullOrEmpty(contentType))
            {
                result.Add(HttpHeaderNames.ContentType, contentType);
            }
            return result;
        }
        #endregion
    }

    public static class MIMEManager
    {
        private static readonly Dictionary<string, string> _mimeDic = new Dictionary<string, string>();
        static MIMEManager()
        {
            //mimeDic.Add(".html", "text/html");
            //mimeDic.Add(".js", "application/javascript");
            //mimeDic.Add(".css", "text/css");

            string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mimeConfig.json");
            if (!File.Exists(configFilePath)) throw new Exception("mimeConfig.json文件丢失");
            string jsonConfigString = File.ReadAllText(configFilePath);
            _mimeDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonConfigString);
        }
        /// <summary>
        /// 获得ContentType
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static string GetContentType(string extension)
        {
            return _mimeDic.ContainsKey(extension) ? _mimeDic[extension] : "application/octet-stream";
        }
    }
}
