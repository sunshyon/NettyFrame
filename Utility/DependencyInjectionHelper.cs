using Microsoft.Extensions.DependencyInjection;
using System;

namespace Utility
{
    public static class DependencyInjectionHelper
    {
        private static IServiceCollection _services = new ServiceCollection();
        private static IServiceProvider _serviceProvider;

        /// <summary>
        /// 注册/构建服务
        /// </summary>
        /// <param name="actions"></param>
        public static void RegistServices(Action<IServiceCollection>[] actions)
        {
            foreach (Action<IServiceCollection> action in actions)
            {
                action?.Invoke(_services);
            }
        }
        public static void RegistServices(Action<IServiceCollection> action)
        {
            action?.Invoke(_services);
        }
        /// <summary>
        /// 构建服务
        /// </summary>
        public static void BuildServices()
        {
            if (_services.Count > 0)
            {
                _serviceProvider = _services.BuildServiceProvider();
            }
            else
            {
                ConsoleHelper.WriteErrorLine("Services Error", "未注册服务");
            }
        }
        /// <summary>
        /// 获得服务
        /// </summary>
        public static object GetService(Type type)
        {
            return _serviceProvider.GetService(type);
        }
        /// <summary>
        /// 获得服务
        /// </summary>
        public static T GetService<T>()
        {
            return _serviceProvider.GetService<T>();
        }
    }
}
