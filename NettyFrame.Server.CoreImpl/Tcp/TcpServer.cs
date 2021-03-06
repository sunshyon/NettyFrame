﻿using DotNetty.Codecs.Http;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using NettyFrame.Common;
using NettyFrame.Server.Interface;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Utility;

namespace NettyFrame.Server.CoreImpl
{
    public class TcpServer : IDotNettyServer
    {
        public event Action<string> OnMessage;
        public event Action<string, string> OnSubMessage;
        public event Action<Exception> OnException;
        public event Func<string> OnGetCommand;

        public async Task RunServerAsync()
        {
            OnSubMessage?.Invoke("服务启动中......", "重要");
            //第一步：创建ServerBootstrap实例
            var bootstrap = new ServerBootstrap();
            //第二步：绑定事件组
            IEventLoopGroup mainGroup = new MultithreadEventLoopGroup(1);//主工作线程组
            IEventLoopGroup workGroup = new MultithreadEventLoopGroup();//工作线程组
            bootstrap.Group(mainGroup, workGroup);
            //第三步：绑定服务端的通道
            bootstrap.Channel<TcpServerSocketChannel>();// 设置通道模式为TcpSocket
            //第四步：配置处理器
            bootstrap.Option(ChannelOption.SoBacklog, 8192);
            bootstrap.ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
            {
                IChannelPipeline pipeline = channel.Pipeline;
                //pipeline.AddLast(new HttpServerCodec());
                //pipeline.AddLast(new HttpObjectAggregator(65536));
                pipeline.AddLast(new TcpServerHandler());
            }));
            //第五步：配置主机和端口号
            string hostName = Dns.GetHostName();
            IPAddress[] ipAddresses = Dns.GetHostAddresses(hostName);
            ipAddresses = ipAddresses.Where(m => m.ToString().IsIPv4()).ToArray();
            var host = ConfigHelper.Configuration["ServerConfig:Host"];
            bool trueAddress = ipAddresses.Any(m => host.Equals(m.ToString()));
            IPAddress iPAddress = trueAddress ? IPAddress.Parse(host) : ipAddresses[0];
            var port = ConfigHelper.Configuration["ServerConfig:Port"];
            IChannel bootstrapChannel = await bootstrap.BindAsync(iPAddress,  int.Parse(port));
            OnSubMessage?.Invoke("服务启动成功", "重要");
            OnMessage?.Invoke($"已监听http://{iPAddress}:{int.Parse(port)}");
            //第六步：停止服务
            OnMessage?.Invoke("输入Stop停止服务");
            string inputKey = string.Empty;
            while (!string.Equals(inputKey, "Stop", StringComparison.Ordinal))
            {
                inputKey = OnGetCommand?.Invoke();
                if (!string.Equals(inputKey, "Stop", StringComparison.Ordinal))
                {
                    OnException?.Invoke(new Exception("未识别命令请重新输入"));
                }
            }
            OnSubMessage?.Invoke("正在停止服务......", "重要");
            await bootstrapChannel.CloseAsync();
            OnSubMessage?.Invoke("服务已停止", "重要");
        }


        private async Task TestRun()
        {
            await Task.Run(() =>
            {
                OnSubMessage?.Invoke("服务启动成功", "重要");
                OnMessage?.Invoke("输入Stop停止服务");
                string inputKey = string.Empty;
                while (!string.Equals(inputKey, "Stop", StringComparison.Ordinal))
                {
                    inputKey = OnGetCommand?.Invoke();
                    if (!string.Equals(inputKey, "Stop", StringComparison.Ordinal))
                    {
                        OnException?.Invoke(new Exception("未识别命令请重新输入"));
                    }
                }
                OnSubMessage?.Invoke("正在停止服务", "重要");
                OnSubMessage?.Invoke("服务已停止", "重要");
            });
        }
    }
}
