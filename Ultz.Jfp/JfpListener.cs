using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Ultz.Jfp
{
    public delegate Stream StreamDecorator(Stream baseStream);
    public class JfpListener
    {
        public JfpListener(TcpListener tcpListener)
        {
            Server = tcpListener;
            Decorator = stream => stream;
        }
        
        public TcpListener Server { get; }
        public StreamDecorator Decorator { get; set; }

        public JfpPump AcceptJfpPump()
        {
            var pump= new JfpPump(Decorator(Server.AcceptTcpClient().GetStream()));
            pump.Start();
            return pump;
        }

        public async Task<JfpPump> AcceptJfpPumpAsync()
        {
            var pump= new JfpPump(Decorator((await Server.AcceptTcpClientAsync()).GetStream()));
            pump.Start();
            return pump;
        }

        public JfpListenerContext AcceptContext()
        {
            var client = Server.AcceptTcpClient();
            var pump= new JfpPump(Decorator(client.GetStream()));
            pump.Start();
            return new JfpListenerContext(pump, client);
        }

        public async Task<JfpListenerContext> AcceptContextAsync()
        {
            var client = await Server.AcceptTcpClientAsync();
            var pump= new JfpPump(Decorator(client.GetStream()));
            pump.Start();
            return new JfpListenerContext(pump, client);
        }

        public void Start()
        {
            Server.Start();
        }

        public void Stop()
        {
            Server.Stop();
        }
    }
}