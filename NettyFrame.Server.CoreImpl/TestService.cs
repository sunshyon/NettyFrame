using NettyFrame.Server.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace NettyFrame.Server.CoreImpl
{
    public class TestService : ITestService
    {
        public void SayHello()
        {
            ConsoleHelper.WriteLine("Test", "Hello", ConsoleColor.Green);
        }
    }
}
