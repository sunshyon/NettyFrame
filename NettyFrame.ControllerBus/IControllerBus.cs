using System;

namespace NettyFrame.ControllerBus
{
    public interface IControllerBus
    {
        /// <summary>
        /// 获取控制器
        /// </summary>
        BaseController GetController(string key);
    }
}
