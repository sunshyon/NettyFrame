using DotNetty.Buffers;
using DotNetty.Codecs.Http;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NettyFrame.Server.CoreImpl.Http
{
    public class WebApiHandler : HttpHandlerContext
    {
        public override async Task HandlerAsync(IChannelHandlerContext ctx, IByteBufferHolder byteBufferHolder)
        {
            await base.HandlerAsync<IFullHttpRequest>(ctx, byteBufferHolder, HandlerRequestAsync);
        }
        #region 私有方法
        /// <summary>
        /// 处理请求
        /// </summary>
        private async Task HandlerRequestAsync(IChannelHandlerContext ctx, IFullHttpRequest request)
        {
            await Task.Run(() => { });
        }
        #endregion
    }
}
