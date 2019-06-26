using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Ultz.Jfp
{
    [PublicAPI]
    public class JfpMessage
    {
        [JsonProperty("id"), PublicAPI]
        public uint Id { get; set; }
        /// <summary>
        /// True if this message is a response to the message with the same <see cref="Id"/>
        /// </summary>
        [JsonProperty("response"), PublicAPI]
        public bool IsResponse { get; set; }
        [JsonProperty("type"), PublicAPI, NotNull]
        public string MessageType { get; set; }
        [JsonProperty("message"), PublicAPI, CanBeNull]
        public byte[] Message { get; set; }
        /// <summary>
        /// If this is true, no more messages with the <see cref="Id"/> should be sent.
        /// </summary>
        [JsonProperty("close"), PublicAPI]
        public bool Close { get; set; }
    }
}