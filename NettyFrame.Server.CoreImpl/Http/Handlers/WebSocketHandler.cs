using DotNetty.Buffers;
using DotNetty.Codecs.Http.WebSockets;
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
        public override async Task HandlerAsync(IChannelHandlerContext ctx, IByteBufferHolder byteBufferHolder)
        {
            await base.HandlerAsync<WebSocketFrame>(ctx, byteBufferHolder, HandlerRequestAsync);
        }
        #region 私有方法
        /// <summary>
        /// 处理请求
        /// </summary>
        private async Task HandlerRequestAsync(IChannelHandlerContext ctx, WebSocketFrame frame)
        {
            await Task.Run(() => { });
        }
        #endregion
    }
}
