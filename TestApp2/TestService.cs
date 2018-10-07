using Ultz.Jfp;
using Ultz.Jfp.SimpleServer;
using Ultz.SimpleServer.Common;
using Ultz.SimpleServer.Internals;

namespace TestApp2
{
    public class TestService : Service
    {
        protected override void BeforeStart()
        {
            
        }

        protected override void OnStart()
        {
        }

        protected override void AfterStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override void BeforeStop()
        {
        }

        protected override void AfterStop()
        {
        }

        protected override void OnError(ErrorType type, IContext context)
        {
        }

        public override IProtocol Protocol => new JfpProtocol();
    }
}