using System;
using System.IO;

namespace Ultz.Jfp
{
    public class OnCommandEventArgs : EventArgs
    {
        public long Id { get; set; }
        public Stream DataStream { get; set; }
        public string MessageType { get; set; }
        public JfpMessage InitiatingMessage { get; set; }
        public JfpPump ReceivingPump { get; set; }
    }
}