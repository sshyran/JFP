namespace Ultz.Jfp.IO
{
    public class JfpClientboundStream : JfpStream
    {
        public JfpClientboundStream(JfpMessagePump pump, long id, string type) : base(pump, id, type)
        {
        }

        public override bool IsResponse => true;
    }
}