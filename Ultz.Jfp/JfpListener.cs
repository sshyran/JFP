using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Ultz.Jfp
{
    public class JfpListener
    {
        public JfpListener(TcpListener tcpListener)
        {
            Server = tcpListener;
        }
        
        public TcpListener Server { get; }

        public JfpPump AcceptJfpPump()
        {
            var pump= new JfpPump(Server.AcceptTcpClient().GetStream());
            pump.Start();
            return pump;
        }

        public async Task<JfpPump> AcceptJfpPumpAsync()
        {
            var pump= new JfpPump((await Server.AcceptTcpClientAsync()).GetStream());
            pump.Start();
            return pump;
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