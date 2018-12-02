using System.Threading.Tasks;

namespace Ultz.Jfp
{
    public interface IJfpListener
    {
        JfpPump AcceptJfpPump();
        Task<JfpPump> AcceptJfpPumpAsync();
        JfpListenerContext AcceptContext();
        Task<JfpListenerContext> AcceptContextAsync();
        void Start();
        void Stop();
    }
}