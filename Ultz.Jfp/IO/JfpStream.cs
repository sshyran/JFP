using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;

namespace Ultz.Jfp.IO
{
    public abstract class JfpStream : Stream
    {
        private JfpMessagePump _pump;
        public MemoryStream _memoryStream;
        private int _currentOffset = 0;
        private bool _closed = false;

        public JfpStream(JfpMessagePump pump, long id, string type)
        {
            _pump = pump;
            Id = id;
            MessageType = type;
            _memoryStream = new MemoryStream();
        }

        public long Id { get; }
        public string MessageType { get; }
        public abstract bool IsResponse { get; }

        private JfpMessage ToResponse(byte[] bytes)
        {
            return new JfpMessage()
                {Close = false, Id = Id, IsResponse = IsResponse, Message = bytes, MessageType = MessageType};
        }

        internal void GotMessage(JfpMessage message)
        {
            if (_closed)
                return; // just ignore it, we don't want the pump to break
            var before = _memoryStream.Position;
            _memoryStream.Write(message.Message,0,message.Message.Length);
            _memoryStream.Seek(before, SeekOrigin.Begin);
        }

        public override void Flush()
        {
            // does nothing
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _memoryStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new System.NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new System.NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_closed)
                throw new ObjectDisposedException("Stream has been closed",(Exception)null);
            _pump.Send(ToResponse(new ArraySegment<byte>(buffer, offset, count).ToArray()));
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override void Close()
        {
            _pump.Send(new JfpMessage(){Close = true, Id = Id, IsResponse = IsResponse, MessageType = MessageType, Message = null});
            _closed = true;
            _memoryStream.Close();
            base.Close();
        }
    }
}