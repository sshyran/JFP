using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Ultz.Jfp.IO
{
    public class JfpMessagePump : IDisposable
    {
        public Stream Stream { get; }
        private StreamWriter _writer;
        private StreamReader _reader;
        private Dictionary<long, JfpServerboundStream> _serverboundMessageStreams = new Dictionary<long, JfpServerboundStream>();
        private Dictionary<long, JfpClientboundStream> _clientboundMessageStreams = new Dictionary<long, JfpClientboundStream>();
        internal JfpMessagePump(Stream stream)
        {
            Stream = stream;
            _writer = new StreamWriter(Stream, Encoding.UTF8) {AutoFlush = true};
            _reader = new StreamReader(Stream,Encoding.UTF8);
        }

        public event EventHandler<JfpNewCommandStreamEventArgs> NewCommandStream; 

        public void HandleMessage(JfpMessage msg)
        {
            if (msg == null)
                return;
            if (msg.IsResponse)
            {
                if (!_serverboundMessageStreams.ContainsKey(msg.Id))
                {
                    throw new InvalidOperationException("Response received for an unknown Id");
                }
                _serverboundMessageStreams[msg.Id].GotMessage(msg);
            }
            else
            {
                if (!_clientboundMessageStreams.ContainsKey(msg.Id))
                {
                    _clientboundMessageStreams.Add(msg.Id, new JfpClientboundStream(this,msg.Id,msg.MessageType));
                    _clientboundMessageStreams[msg.Id].GotMessage(msg);
                    NewCommandStream?.Invoke(this,new JfpNewCommandStreamEventArgs(){Message = msg});
                    return;
                }
                _clientboundMessageStreams[msg.Id].GotMessage(msg);
            }
        }

        public Stream GetStreamByRemoteId(long id)
        {
            return _clientboundMessageStreams[id];
        }

        public Stream GetStreamByClientId(long id)
        {
            return _serverboundMessageStreams[id];
        }

        internal void Send(JfpMessage msg)
        {
            if (!msg.IsResponse && !_serverboundMessageStreams.ContainsKey(msg.Id))
                _serverboundMessageStreams.Add(msg.Id, new JfpServerboundStream(this,msg.Id,msg.MessageType));
            _writer.WriteLine(JsonConvert.SerializeObject(msg));
        }

        internal JfpMessage Receive()
        {
            var line = _reader.ReadLine();
            if (line == null)
                return null;
            return JsonConvert.DeserializeObject<JfpMessage>(line);
        }
        
        internal async Task<JfpMessage> ReceiveAsync()
        {
            var line = await _reader.ReadLineAsync();
            if (line == null)
                return null;
            return JsonConvert.DeserializeObject<JfpMessage>(line);
        }

        public void Dispose()
        {
            Stream?.Dispose();
            _writer?.Dispose();
            _reader?.Dispose();
            _serverboundMessageStreams = new Dictionary<long, JfpServerboundStream>();
            _clientboundMessageStreams = new Dictionary<long, JfpClientboundStream>();
        }
    }
}