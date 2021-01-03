﻿using Microsoft.Extensions.DependencyInjection;
using NettyFrame.Server.CoreImpl;
using NettyFrame.Server.Interface;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Utility;

namespace NettyFrame.Server
{
    class Program
    {
        static  async Task Main(string[] args)
        {
            await StartServer();
        }
        #region 私有方法
        /// <summary>
        /// 注册服务
        /// </summary>
        private static bool TryRegisterService()
        {
            try
            {
                DependencyInjectionHelper.RegistServices(s => s.AddScoped<IDotNettyServer, DotNettyServer>());
                DependencyInjectionHelper.RegistServices(s => s.AddScoped<ITestService, TestService>());
                DependencyInjectionHelper.BuildServices();
                return true;
            }
            catch (Exception e)
            {
                ConsoleHelper.WriteErrorLine("Error",e.Message);
                return false;
            }
        }
        private static async Task TestService()
        {
            var test= DependencyInjectionHelper.GetService<ITestService>();
            test.SayHello();
        }
        /// <summary>
        /// 测试启动服务
        /// </summary>
        /// <returns></returns>
        private static async Task StartServer()
        {
            string version = Assembly.GetCallingAssembly().GetName().Version.ToString();
            Console.Title = $"NettyFrame.Server [版本号:{version}]";
            if (TryRegisterService())
            {
                try
                {
                    //TestService();

                    var dotNettyServer = DependencyInjectionHelper.GetService<IDotNettyServer>();
                    dotNettyServer.OnException += ConsoleHelper.ServerWriteError;
                    dotNettyServer.OnGetCommand += Console.ReadLine;
                    dotNettyServer.OnMessage += mesage => ConsoleHelper.WriteLine("Message -> ",mesage);
                    dotNettyServer.OnSubMessage += (message, subTitle) => ConsoleHelper.WriteLine(message, subTitle);
                    await dotNettyServer.RunServerAsync();
                }
                catch (Exception e)
                {
                    ConsoleHelper.WriteErrorLine("服务器发生致命错误 -> ", e.Message);
                }
            }
            else
            {
                ConsoleHelper.WriteErrorLine("注册服务失败", "失败");
            }
        }
        #endregion
    }

}