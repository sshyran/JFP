using System.Threading.Tasks;
using Ultz.SimpleServer.Internals;

namespace Ultz.Jfp.SimpleServer
{
    public class JfpConnectionHandler : IJfpListener
    {
        public IListener Listener { get; }
        public JfpConnectionHandler(IListener listener)
        {
            Listener = listener;
        }
        
        public JfpPump AcceptJfpPump()
        {
            var pump= new JfpPump(Listener.Accept().Stream);
            pump.Start();
            return pump;
        }

        public async Task<JfpPump> AcceptJfpPumpAsync()
        {
            var pump= new JfpPump((await Listener.AcceptAsync()).Stream);
            pump.Start();
            return pump;
        }

        public JfpListenerContext AcceptContext()
        {
            var client = new ConnectionClient(Listener.Accept());
            var pump= new JfpPump(client.Stream);
            pump.Start();
            return new JfpListenerContext(pump,client);
        }

        public async Task<JfpListenerContext> AcceptContextAsync()
        {
            var client = new ConnectionClient(await Listener.AcceptAsync());
            var pump= new JfpPump(client.Stream);
            pump.Start();
            return new JfpListenerContext(pump,client);
        }

        public void Start()
        {
            Listener.Start();
        }

        public void Stop()
        {
            Listener.Stop();
        }
    }
}