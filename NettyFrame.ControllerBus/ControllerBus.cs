using System;
using System.Collections.Generic;
using System.Text;

namespace NettyFrame.ControllerBus
{
    public class ControllerBus : IControllerBus
    {
        private readonly IServiceProvider _serviceProvider;
        public ControllerBus()
        {
        }
        public ControllerBus(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public BaseController GetController(string key)
        {
            if (!_controller.ContainsKey(key)) throw new Exception("未找到对应控制器");
            var type = _controller[key];
            var controller = _serviceProvider.GetService(type);
            if (controller == null) throw new Exception($"未找到控制器{type.FullName}");
            if (!(controller is BaseController baseController)) throw new Exception($"控制器必须继承类{nameof(BaseController)}");
            return baseController;

        }

        /// <summary>
        /// 控制器类型字典
        /// </summary>
        private static readonly Dictionary<string, Type> _controller = new Dictionary<string, Type>();
        /// <summary>
        /// 添加控制器类型
        /// </summary>
        public static bool TryAddController(Type type)
        {
            if (type == null) throw new Exception("控制器类型为空");
            if (!type.IsSubclassOf(typeof(BaseController))) throw new Exception($"控制器必须继承类{nameof(BaseController)}");
            string key = type.Name;
            if (key.EndsWith("Controller")) key = key.Substring(0, key.Length - 10);
            if (_controller.ContainsKey(key)) return false;
            _controller.Add(key, type);
            return true;
        }
    }
}
