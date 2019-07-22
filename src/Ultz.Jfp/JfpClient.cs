using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Ultz.Jfp
{
    [PublicAPI]
    public class JfpClient : IDisposable
    {
        private StreamReader _streamReader;
        private StreamWriter _streamWriter;
        private string _cachedMessage;
        [PublicAPI]
        public Stream BaseStream => _streamReader.BaseStream;
        [PublicAPI]
        public JfpClient([NotNull] Stream stream, JfpProtocol protocol = JfpProtocol.JfpOrLjfp)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            if (stream.CanRead)
            {
                _streamReader = new StreamReader(stream);
            }

            if (stream.CanWrite)
            {
                _streamWriter = new StreamWriter(stream);
            }

            SupportedProtocol = protocol;
            if (stream.CanRead && stream.CanWrite)
            {
                CurrentProtocol = GetProtocol(this);
            }
            else
            {
                CurrentProtocol = JfpProtocol.Jfp;
            }
        }

        internal static JfpProtocol GetProtocol(JfpClient client)
        {
            client._streamWriter.WriteLine((byte)client.SupportedProtocol);
            var remoteProtocolLine = client._streamReader.ReadLine();
            JfpProtocol remoteProtocol;
            if (remoteProtocolLine?.StartsWith("{") ?? true)
            {
                // this instance doesn't support negotiation or we've reached the end of the stream - use jfp only.
                remoteProtocol = JfpProtocol.Jfp;
                client._cachedMessage = remoteProtocolLine;
            }
            else
            {
                remoteProtocol = (JfpProtocol) (byte) remoteProtocolLine[0];
            }

            var finalProtocol = Jfp.GetProtocol(client.SupportedProtocol, remoteProtocol);
            if (finalProtocol == null)
            {
                throw new JfpNegotiationException("Failed to negotiate a protocol. The remote wanted to use " +
                                                  remoteProtocol + ", but this client only supports "
                                                  + client.SupportedProtocol);
            }

            return finalProtocol.Value;
        }

        public JfpProtocol SupportedProtocol { get; }
        public JfpProtocol CurrentProtocol { get; }

        [PublicAPI, CanBeNull]
        public JfpMessage ReceiveMessage()
        {
            if (_streamReader == null)
            {
                throw new NotSupportedException("Underlying stream does not support reading.");
            }

            if (_cachedMessage != null)
            {
                var msg = JsonConvert.DeserializeObject<JfpMessage>(_cachedMessage);
                _cachedMessage = null;
                return msg;
            }

            var line = _streamReader.ReadLine();
            return CurrentProtocol == JfpProtocol.Jfp
                ? JsonConvert.DeserializeObject<JfpMessage>(line ?? "null")
                : LReceiveMessage(line?.Cast<byte>());
        }

        private JfpMessage LReceiveMessage(IEnumerable<byte> bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            // AAAABCDDDDX...EEEEY...
            // where AAAA = id
            // where B = close
            // where C = response
            // where DDDD = length of type
            // where X... = type
            // where EEEE = length of message
            // where Y... = message
            var asArray = bytes.ToArray();
            var id = BitConverter.ToUInt32(new ArraySegment<byte>(asArray, 0, 4).ToArray(), 0);
            var isResponse = asArray[4] == 1;
            var close = asArray[5] == 1;
            var typeLen = BitConverter.ToInt32(new ArraySegment<byte>(asArray, 6, 4).ToArray(), 0);
            var type = new string(new ArraySegment<byte>(asArray, 10, typeLen).Cast<char>().ToArray());
            var msgLen = BitConverter.ToInt32(new ArraySegment<byte>(asArray, 10 + typeLen, 4).ToArray(), 0);
            var msg = new ArraySegment<byte>(asArray, 14 + typeLen, msgLen).ToArray();
            return new JfpMessage {Close = close, Id = id, IsResponse = isResponse, Message = msg, MessageType = type};
        }

        [PublicAPI, CanBeNull]
        public async Task<JfpMessage> ReceiveMessageAsync()
        {
            if (_streamReader == null)
            {
                throw new NotSupportedException("Underlying stream does not support reading.");
            }

            if (_cachedMessage != null)
            {
                var msg = JsonConvert.DeserializeObject<JfpMessage>(_cachedMessage);
                _cachedMessage = null;
                return msg;
            }

            var line = await _streamReader.ReadLineAsync();
            return CurrentProtocol == JfpProtocol.Jfp
                ? JsonConvert.DeserializeObject<JfpMessage>(line ?? "null")
                : LReceiveMessage(line?.Cast<byte>());
        }

        [PublicAPI]
        public void SendMessage([NotNull] JfpMessage message)
        {
            if (_streamWriter == null)
            {
                throw new NotSupportedException("Underlying stream does not support writing.");
            }
            _streamWriter.WriteLine(JsonConvert.SerializeObject(message));
        }

        [PublicAPI]
        public async Task SendMessageAsync([NotNull] JfpMessage message)
        {
            if (_streamReader == null)
            {
                throw new NotSupportedException("Underlying stream does not support writing.");
            }
            await _streamWriter.WriteLineAsync(JsonConvert.SerializeObject(message));
        }

        public void Dispose()
        {
            _streamReader?.Dispose();
            _streamWriter?.Dispose();
        }
    }
}
