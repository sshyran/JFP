using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace Ultz.Jfp.Tests
{
    public class JfpClientTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public JfpClientTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public Stream Stream => Assembly.GetAssembly(typeof(JfpClientTests))
            .GetManifestResourceStream("Ultz.Jfp.Tests.JfpClientTests.txt");

        [Fact]
        public void CanReceive()
        {
            var jfpClient = new JfpClient(Stream);
            var msg = jfpClient.ReceiveMessage();
            _testOutputHelper.WriteLine(JsonConvert.SerializeObject(msg));
            Assert.True(msg.Message != null && new byte[] {0x48, 0x65, 0x6c, 0x6c, 0x6f}.SequenceEqual(msg.Message));
        }

        [Fact]
        public void CanSend()
        {
            var stream = File.OpenWrite("canSend.txt");
            var jfpClient = new JfpClient(stream, JfpProtocol.Jfp);
            jfpClient.SendMessage
            (
                new JfpMessage
                {
                    Close = true, Id = 1u, IsResponse = false, Message = new byte[] {0x42, 0x79, 0x65},
                    MessageType = "JfpClientTest"
                }
            );
            stream.Flush();
            jfpClient.Dispose();
            Assert.True
            (
                File.ReadAllLines("canSend.txt")[0] ==
                "{\"id\":1,\"response\":false,\"type\":\"JfpClientTest\",\"message\":\"Qnll\",\"close\":true}"
            );
        }
    }
}
