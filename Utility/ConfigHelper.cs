using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public static class ConfigHelper
    {
        private static IConfiguration _configuration;
        public static IConfiguration Configuration
        {
            get
            {
                if (_configuration == null)
                {
                    ConfigurationBuilder();
                }
                return _configuration;
            }
        }

        private const string DefaultConfigFileName = "appsetting.json";
        /// <summary>
        /// 配置生成
        /// </summary>
        /// <param name="targetConfigFileName"></param>
        private static void ConfigurationBuilder(string targetConfigFileName = null)
        {
            string appConfigFile = string.IsNullOrEmpty(targetConfigFileName) ?
                $"{DefaultConfigFileName}" :$"{targetConfigFileName}";
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile(appConfigFile);
            _configuration = builder.Build();
        }
    }
}
