using DotNetty.Buffers;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NettyFrame.Server.CoreImpl
{
    /// <summary>
    /// 未封装
    /// </summary>
    class DotNettyClient
    {
        static async Task RunClientAsync()
        {
            var group = new MultithreadEventLoopGroup();
            try
            {
                var bootstrap = new Bootstrap();
                bootstrap
                    .Group(group)
                    .Channel<TcpSocketChannel>()
                    .Option(ChannelOption.TcpNodelay, true)
                    .Handler(new ActionChannelInitializer<ISocketChannel>(c =>
                    {
                        IChannelPipeline pipeline = c.Pipeline;
                        pipeline.AddLast(new HelloClientHandler());
                    }));

                IChannel clientChannel = await bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3399));
                Console.ReadLine();
                await clientChannel.CloseAsync();
            }
            finally
            {
                await group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
            }
        }
    }
    class HelloClientHandler : ChannelHandlerAdapter
    {
        public override void ChannelActive(IChannelHandlerContext context)
        {
            Console.WriteLine("我是客户端.");
            Console.WriteLine($"连接至服务端{context}.");

            //编码成IByteBuffer,发送至服务端
            string message = "客户端1";
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            IByteBuffer initialMessage = Unpooled.Buffer(messageBytes.Length);
            initialMessage.WriteBytes(messageBytes);

            context.WriteAndFlushAsync(initialMessage);
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            if (message is IByteBuffer buffer)
            {
                Console.WriteLine($"IByteBuffer方式，从服务端接收:{buffer.ToString(Encoding.UTF8)}");
            }
        }

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

        public override void HandlerAdded(IChannelHandlerContext context)
        {
            Console.WriteLine($"服务端{context}上线.");
            base.HandlerAdded(context);
        }

        public override void HandlerRemoved(IChannelHandlerContext context)
        {
            Console.WriteLine($"服务端{context}下线.");
            base.HandlerRemoved(context);
        }

    }
}
