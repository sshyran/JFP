using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Ultz.Jfp
{
    [PublicAPI]
    public delegate Stream StreamDecorator(Stream baseStream);
    [PublicAPI]
    public class JfpListener
    {
        [PublicAPI]
        public JfpListener([NotNull] TcpListener tcpListener)
        {
            Server = tcpListener;
            Decorator = stream => stream;
        }
        [PublicAPI, NotNull]
        public TcpListener Server { get; }
        [PublicAPI, NotNull]
        public StreamDecorator Decorator { get; set; }
        [PublicAPI]
        public JfpClient AcceptJfpClient()
        {
            return new JfpClient(Decorator(Server.AcceptTcpClient().GetStream()));
        }
        [PublicAPI]
        public async Task<JfpClient> AcceptJfpClientAsync()
        {
            return new JfpClient(Decorator((await Server.AcceptTcpClientAsync()).GetStream()));
        }
        [PublicAPI]
        public void Start()
        {
            Server.Start();
        }
        [PublicAPI]
        public void Stop()
        {
            Server.Stop();
        }
    }
}