using System;

namespace Ultz.Jfp.IO
{
    public class JfpNewCommandStreamEventArgs : EventArgs
    {
        public JfpMessage Message { get; set; }
    }
}