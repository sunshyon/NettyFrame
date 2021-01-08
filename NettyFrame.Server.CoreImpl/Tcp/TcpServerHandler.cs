using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NettyFrame.Server.CoreImpl
{
    public class TcpServerHandler : ChannelHandlerAdapter //管道处理基类，较常用
    {
        public override bool IsSharable => true;//标注一个channel handler可以被多个channel安全地共享。

        //  重写基类的方法，当消息到达时触发，这里收到消息后，在控制台输出收到的内容，并原样返回了客户端
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            if (message is IByteBuffer buffer)
            {
                Console.WriteLine("从客户端接收: " + buffer.ToString(Encoding.UTF8));
            }

            //编码成IByteBuffer,发送至客户端
            string msg = "服务端从客户端接收到内容后返回，我是服务端";
            byte[] messageBytes = Encoding.UTF8.GetBytes(msg);
            IByteBuffer initialMessage = Unpooled.Buffer(messageBytes.Length);
            initialMessage.WriteBytes(messageBytes);

            context.WriteAsync(initialMessage);//写入输出流
        }

        // 输出到客户端，也可以在上面的方法中直接调用WriteAndFlushAsync方法直接输出
        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

        //捕获 异常，并输出到控制台后断开链接，提示：客户端意外断开链接，也会触发
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine("异常: " + exception);
            context.CloseAsync();
        }

        //客户端连接进来时
        public override void HandlerAdded(IChannelHandlerContext context)
        {
            Console.WriteLine($"客户端{context}上线.");
            base.HandlerAdded(context);
        }

        //客户端下线断线时
        public override void HandlerRemoved(IChannelHandlerContext context)
        {
            Console.WriteLine($"客户端{context}下线.");
            base.HandlerRemoved(context);
        }

        //服务器监听到客户端活动时
        public override void ChannelActive(IChannelHandlerContext context)
        {
            Console.WriteLine($"客户端{context.Channel.RemoteAddress}在线.");
            base.ChannelActive(context);
        }

        //服务器监听到客户端不活动时
        public override void ChannelInactive(IChannelHandlerContext context)
        {
            Console.WriteLine($"客户端{context.Channel.RemoteAddress}离线了.");
            base.ChannelInactive(context);
        }

    }

}
