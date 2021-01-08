using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Codecs.Http;

namespace NettyFrame.Server.Interface
{
    public interface IHttpHandler
    {
        IFullHttpResponse GetHttpResponse(IFullHttpRequest request);
    }
}
