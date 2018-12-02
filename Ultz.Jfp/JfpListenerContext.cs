using System.Net.Sockets;

namespace Ultz.Jfp
{
    public class JfpListenerContext
    {
        public JfpListenerContext(JfpPump pump, IJfpClient client)
        {
            Pump = pump;
            Client = client;
        }
        public JfpPump Pump { get; }
        public IJfpClient Client { get; }
    }
}