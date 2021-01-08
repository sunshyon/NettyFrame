using DotNetty.Buffers;
using DotNetty.Codecs.Http;
using DotNetty.Common.Utilities;
using NettyFrame.Server.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NettyFrame.Server.CoreImpl.Http
{
    public class HttpHandler : IHttpHandler
    {
        public IFullHttpResponse GetHttpResponse(IFullHttpRequest request)
        {
            if (!request.Result.IsSuccess)
            {
                return GetHttpResponse(HttpResponseStatus.BadRequest);
            }
            return GetHttpResponse(HttpResponseStatus.OK, "Hello World!");
        }

        #region 私有方法
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
}
