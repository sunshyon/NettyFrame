using DotNetty.Buffers;
using DotNetty.Codecs.Http;
using DotNetty.Transport.Channels;
using NettyFrame.Server.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NettyFrame.Server.CoreImpl.Http
{
    public class HttpChannelHandler : SimpleChannelInboundHandler<IByteBufferHolder>
    {
        protected override void ChannelRead0(IChannelHandlerContext ctx, IByteBufferHolder msg)
        {
            Task.Run(async () => await GetHandler().HandlerAsync(ctx, msg));

        }
        /// <summary>
        /// 获得处理器
        /// </summary>
        private HandlerContext GetHandler()
        {
            var handlers = new HandlerContext[]
            {
                new WebSocketHandler(),
                new WebApiHandler(),
                new FileHandler()
            };
            for (var i = 1; i < handlers.Length; i++)
            {
                handlers[i - 1].SetNext(handlers[i]);
            }
            return handlers[0];
        }
    }

    #region Old
    //public class HttpChannelHandler : SimpleChannelInboundHandler<object>
    //{
    //    private readonly IHttpHandler _httpHandler;
    //    public HttpChannelHandler(IHttpHandler httpHandler)
    //    {
    //        _httpHandler = httpHandler;
    //    }
    //    protected override void ChannelRead0(IChannelHandlerContext ctx, object msg)
    //    {
    //        if (msg is IFullHttpRequest request)
    //        {
    //            Task.Run(async () =>
    //            {
    //                IFullHttpResponse response = await _httpHandler.GetHttpResponse(request);
    //                await SendHttpResponseAsync(ctx, response);
    //            });
    //        }
    //    }
    //    /// <summary>
    //    /// 发送Http返回
    //    /// </summary>
    //    /// <param name="ctx"></param>
    //    /// <param name="response"></param>
    //    private async Task SendHttpResponseAsync(IChannelHandlerContext ctx, IFullHttpResponse response)
    //    {
    //        await ctx.Channel.WriteAndFlushAsync(response);
    //        await ctx.CloseAsync();
    //    }
    //} 
    #endregion
}
