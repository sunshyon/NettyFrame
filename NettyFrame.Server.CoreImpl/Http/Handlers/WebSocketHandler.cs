using DotNetty.Buffers;
using DotNetty.Codecs.Http;
using DotNetty.Codecs.Http.WebSockets;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NettyFrame.Server.CoreImpl.Http
{
    public class WebSocketHandler : HandlerContext
    {
        private const string WebSocketUrl = "/websocket";
        private WebSocketServerHandshaker _handShaker;
        public override async Task HandlerAsync(IChannelHandlerContext ctx, IByteBufferHolder byteBufferHolder)
        {
            if (byteBufferHolder is IFullHttpRequest request && request.Uri.Equals(WebSocketUrl, StringComparison.OrdinalIgnoreCase))
            {
                await ProtocolUpdateAsync(ctx, request);
            }
            else
            {
                await base.HandlerAsync<WebSocketFrame>(ctx, byteBufferHolder, HandlerRequestAsync);
            }
        }
        #region 私有方法
        /// <summary>
        /// 协议升级
        /// </summary>
        private async Task ProtocolUpdateAsync(IChannelHandlerContext ctx, IFullHttpRequest request)
        {
            string address = GetWebSocketAddress(request);
            var webSocketServerHandshakerFactory = new WebSocketServerHandshakerFactory(address, null, true);
            _handShaker = webSocketServerHandshakerFactory.NewHandshaker(request);
            if (_handShaker == null)
            {
                await WebSocketServerHandshakerFactory.SendUnsupportedVersionResponse(ctx.Channel);
            }
            else
            {
                await _handShaker.HandshakeAsync(ctx.Channel, request);
            }
        }
        /// <summary>
        /// 获取WebSocket地址
        /// </summary>
        private string GetWebSocketAddress(IFullHttpRequest request)
        {
            request.Headers.TryGet(HttpHeaderNames.Host, out ICharSequence value);
            string address = "ws://" + value.ToString() + WebSocketUrl;
            return address;
        }
        /// <summary>
        /// 处理请求
        /// </summary>
        private async Task HandlerRequestAsync(IChannelHandlerContext ctx, WebSocketFrame frame)
        {
            switch (frame)
            {
                //关闭
                case CloseWebSocketFrame closeWebSocketFrame:
                    if (_handShaker == null) return;
                    await _handShaker.CloseAsync(ctx.Channel, closeWebSocketFrame);
                    break;
                //心跳->Ping
                case PingWebSocketFrame _:
                    await ctx.WriteAndFlushAsync(new PongWebSocketFrame());
                    break;
                //心跳->Poing
                case PongWebSocketFrame _:
                    await ctx.WriteAndFlushAsync(new PingWebSocketFrame());
                    break;
                //文本
                case TextWebSocketFrame textWebSocketFrame:
                    await ctx.WriteAndFlushAsync(new TextWebSocketFrame($"服务器返回消息:{textWebSocketFrame.Content.ToString(Encoding.UTF8)}"));
                    break;
                //二进制
                case BinaryWebSocketFrame binaryWebSocketFrame:
                    await ctx.WriteAndFlushAsync(new BinaryWebSocketFrame(binaryWebSocketFrame.Content));
                    break;
            }
        }
        #endregion
    }
}
