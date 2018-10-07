namespace Ultz.Jfp.IO
{
    /// <summary>
    /// A response stream filled as responses are received for commands the client has sent
    /// </summary>
    public class JfpServerboundStream : JfpStream
    {
        public JfpServerboundStream(JfpMessagePump pump, long id, string type) : base(pump, id, type)
        {
        }

        public override bool IsResponse => false;
    }
}