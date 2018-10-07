using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Ultz.Jfp.IO;

namespace Ultz.Jfp
{
    public class JfpPump : IDisposable
    {
        public JfpMessagePump Pump { get; }
        private bool _pumping = false;
        private long _currentId = long.MinValue;
        private Task _receiptPump;
        private CancellationTokenSource _cancellationToken;

        public JfpPump(Stream stream)
        {
            Pump = new JfpMessagePump(stream);
            Pump.NewCommandStream += PumpOnNewCommandStream;
        }

        public event EventHandler<OnCommandEventArgs> OnCommand;

        private void PumpOnNewCommandStream(object sender, JfpNewCommandStreamEventArgs e)
        {
            OnCommand?.Invoke(this,
                new OnCommandEventArgs()
                {
                    DataStream = Pump.GetStreamByRemoteId(e.Message.Id), Id = e.Message.Id,
                    InitiatingMessage = e.Message, MessageType = e.Message.MessageType, ReceivingPump = this
                });
        }

        public void Start()
        {
            _cancellationToken = new CancellationTokenSource();
            _receiptPump = PumpTask().ContinueWith(t => t.GetAwaiter().GetResult(), _cancellationToken.Token);
        }

        public void Stop()
        {
            _pumping = false;
            _cancellationToken.Cancel();
            _receiptPump.Dispose();
        }

        public Stream Send(string command, byte[] data)
        {
            Pump.Send(new JfpMessage()
                {Close = false, Id = _currentId++, IsResponse = false, MessageType = command, Message = data});
            return Pump.GetStreamByClientId(_currentId - 1);
        }

        private Task PumpTask()
        {
            return Task.Run(() =>
            {
                _pumping = true;
                while (_pumping)
                {
                    Pump.HandleMessage(Pump.Receive());
                }
            });
        }

        public void Dispose()
        {
            if (_pumping)
                Stop();
            _receiptPump?.Dispose();
            _cancellationToken?.Dispose();
        }
    }
}