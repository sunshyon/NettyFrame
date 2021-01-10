using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NettyFrame.Server.CoreImpl.Http
{
    public abstract class HandlerContext
    {
        private HandlerContext _handlerContext;
        private bool _canNext = true;
        /// <summary>
        /// 是否可以执行下一步标识
        /// </summary>
        protected bool CanNext => _handlerContext != null && _canNext;

        /// <summary>
        /// 设置下一步
        /// </summary>
        public void SetNext(HandlerContext context)
        {
            _handlerContext = context;
        }
        /// <summary>
        /// 处理
        /// </summary>
        protected async Task HandlerAsync<T>(IChannelHandlerContext ctx, IByteBufferHolder byteBufferHolder, Func<IChannelHandlerContext, T, Task> action)
        {
            if (byteBufferHolder is T target && action != null)
            {
                await action(ctx, target);
            }
            await NextAsync(ctx, byteBufferHolder);
        }
        /// <summary>
        /// 执行下一步
        public async Task NextAsync(IChannelHandlerContext ctx, IByteBufferHolder byteBufferHolder)
        {
            if (CanNext)
            {
                await _handlerContext.HandlerAsync(ctx, byteBufferHolder);
            }
        }
        /// <summary>
        /// 处理(抽象)
        /// </summary>
        public abstract Task HandlerAsync(IChannelHandlerContext ctx, IByteBufferHolder byteBufferHolder);

        /// <summary>
        /// 停止处理
        /// </summary>
        protected void StopHandler()
        {
            _canNext = false;
        }
    }
}
