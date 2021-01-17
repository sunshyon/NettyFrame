using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NettyFrame.ControllerBus
{
    public abstract class BaseController
    {
        public MethodInfo GetAction(string key)
        {
            Type controllerType = GetType();
            MethodInfo methodInfo = controllerType.GetMethod(key);
            if (methodInfo == null)
                throw new Exception("未找到对应Action");
            return methodInfo;
        }
    }
}
