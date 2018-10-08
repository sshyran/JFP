using System.Net.Sockets;

namespace Ultz.Jfp
{
    public class JfpListenerContext
    {
        public JfpListenerContext(JfpPump pump, TcpClient client)
        {
            Pump = pump;
            Client = client;
        }
        public JfpPump Pump { get; }
        public TcpClient Client { get; }
    }
}