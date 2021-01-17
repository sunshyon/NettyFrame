using DotNetty.Buffers;
using DotNetty.Codecs.Http;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using NettyFrame.ControllerBus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace NettyFrame.Server.CoreImpl.Http
{
    public class WebApiHandler : HttpHandlerContext
    {
        private readonly IControllerBus _controllerBus;

        public WebApiHandler(IControllerBus controllerBus)
        {
            _controllerBus = controllerBus;
        }
        public override async Task HandlerAsync(IChannelHandlerContext ctx, IByteBufferHolder byteBufferHolder)
        {
            try
            {
                await base.HandlerAsync<IFullHttpRequest>(ctx, byteBufferHolder, HandlerRequestAsync);
            }
            catch (Exception e)
            {
                ShowException?.Invoke(e);
                var headers = GetDefaultHeaders("text/plain;charset=utf-8");
                IFullHttpResponse response = GetHttpResponse(HttpResponseStatus.InternalServerError, e.Message, headers);
                await SendHttpResponseAsync(ctx, response);
            }
        }
        #region 私有方法
        /// <summary>
        /// 处理请求
        /// </summary>
        private async Task HandlerRequestAsync(IChannelHandlerContext ctx, IFullHttpRequest request)
        {
            if (!request.Uri.StartsWith("/api/"))
                return;
            var urls = request.Uri.Split('/');
            var controllerKey = urls[2];
            var controller = _controllerBus.GetController(controllerKey);
            var actionKey = urls[3].Split('?')[0];
            var action = controller.GetAction(actionKey);

            //处理业务
            IFullHttpResponse response = await GetResponseAsync(request, controller, action);
            await SendHttpResponseAsync(ctx, response);
            StopHandler();
        }
        /// <summary>
        /// 获得Response
        /// </summary>
        private async Task<IFullHttpResponse> GetResponseAsync(IFullHttpRequest request, BaseController baseController, MethodInfo action)
        {
            IFullHttpResponse result = null;
            string bodyParams = GetBodyParams(request);
            Dictionary<string, string> urlParams = GetUrlParams(request);
            ParameterInfo[] parameters = action.GetParameters();
            object[] @params = null;
            if (parameters.Length > 0)
            {
                @params = new object[parameters.Length];
                for (var i = 0; i < parameters.Length; i++)
                {
                    if (!string.IsNullOrEmpty(bodyParams) && parameters[i].ParameterType.IsClass && parameters[i].ParameterType != typeof(string))
                    {
                        @params[i] = bodyParams.JsonToT<object>(parameters[i].ParameterType);
                        continue;
                    }
                    if (urlParams.ContainsKey(parameters[i].Name))
                    {
                        @params[i] = urlParams[parameters[i].Name].ConvertTo(parameters[i].ParameterType);
                        continue;
                    }
                    if (@params[i] == null)
                    {
                        //ResultModel resultModel = ResultModel.Fail($"参数{parameters[i].Name}错误");
                        result = GetHttpResponse(HttpResponseStatus.BadRequest, $"参数{parameters[i].Name}错误)");
                        break;
                    }
                }
            }
            result = result ?? await GetResponseAsync(baseController, action, @params);
            return result;
        }
        /// <summary>
        /// 获得Response
        /// </summary>
        private async Task<IFullHttpResponse> GetResponseAsync(BaseController baseController, MethodInfo action, object[] @params)
        {
            object actionResult = action.Invoke(baseController, @params);
            if (actionResult is Task task)
            {
                dynamic taskObj = task;
                actionResult = await taskObj;
            }
            IFullHttpResponse result;
            if (actionResult != null)
            {
                if (actionResult is Stream stream)
                {
                    result = await GetStreamResponseAsync(stream);
                }
                else if (actionResult.GetType().IsClass && !(actionResult is string))
                {
                    result = GetJsonResponse(actionResult);
                }
                else
                {
                    result = GetTextResponse(actionResult);
                }
            }
            else
            {
                Dictionary<AsciiString, object> headers = GetDefaultHeaders();
                result = GetHttpResponse(HttpResponseStatus.OK, headers);
            }
            return result;
        }
        /// <summary>
        /// 获取流返回
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private async Task<IFullHttpResponse> GetStreamResponseAsync(Stream stream)
        {
            var bytes = new byte[stream.Length];
            await stream.ReadAsync(bytes, 0, bytes.Length);
            stream.Close();
            stream.Dispose();
            Dictionary<AsciiString, object> headers = GetDefaultHeaders("application/octet-stream");
            IFullHttpResponse result = GetHttpResponse(HttpResponseStatus.OK, bytes, headers);
            return result;
        }
        /// <summary>
        /// 获取Json返回
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private IFullHttpResponse GetJsonResponse(object actionResult)
        {
            Dictionary<AsciiString, object> headers = GetDefaultHeaders("application/json;charset=utf-8");
            IFullHttpResponse result = GetHttpResponse(HttpResponseStatus.OK, actionResult.ToJson(), headers);
            return result;
        }
        /// <summary>
        /// 获取文本返回
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private IFullHttpResponse GetTextResponse(object actionResult)
        {
            Dictionary<AsciiString, object> headers = GetDefaultHeaders("text/plain;charset=utf-8");
            IFullHttpResponse result = GetHttpResponse(HttpResponseStatus.OK, actionResult.ToString(), headers);
            return result;
        }

        /// <summary>
        /// 获得Body参数
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string GetBodyParams(IFullHttpRequest request)
        {
            string result = request.Content.ReadString(request.Content.Capacity, Encoding.UTF8);
            return result;
        }
        /// <summary>
        /// 获取Url参数
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetUrlParams(IFullHttpRequest request)
        {
            var result = new Dictionary<string, string>();
            string[] tempString = request.Uri.Split('?');
            if (tempString.Length <= 1) return result;
            string[] paramsString = tempString[1].Split('&');
            foreach (string param in paramsString)
            {
                if (string.IsNullOrEmpty(param)) continue;
                string[] values = param.Split('=');
                if (values.Length != 2 || result.ContainsKey(values[0])) continue;
                result.Add(values[0], values[1]);
            }
            return result;
            #endregion
        }
    }
}
