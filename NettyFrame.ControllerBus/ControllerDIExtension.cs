using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NettyFrame.ControllerBus
{
    public static class ControllerDIExtension
    {
        /// <summary>
        /// 添加控制器总线服务
        /// </summary>
        public static void AddControllerBus(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddSingleton<IControllerBus, ControllerBus>();
            services.AddController(assemblies);
        }
        /// <summary>
        /// 添加控制器服务
        /// </summary>
        private static void AddController(this IServiceCollection services,params Assembly[] assemblies)
        {
            var baseControllerType = typeof(BaseController);
            foreach(var assembly in assemblies)
            {
                foreach(var type in assembly.GetTypes())
                {
                    if (!type.IsSubclassOf(baseControllerType))
                        continue;
                    if (ControllerBus.TryAddController(type))
                    {
                        services.AddTransient(type);
                    }
                }
            }
        }
    }
}
