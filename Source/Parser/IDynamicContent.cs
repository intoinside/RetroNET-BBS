using Encoder;
using System.Net.Sockets;

namespace Parser
{
    public interface IDynamicContent
    {
        Task<string> HandleConnectionFlow(NetworkStream stream, IEncoder encoder);
    }
}
