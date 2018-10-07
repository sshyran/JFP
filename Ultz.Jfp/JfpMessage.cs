using Newtonsoft.Json;

namespace Ultz.Jfp
{
    public class JfpMessage
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        /// <summary>
        /// True if this message is a response to the message with the same <see cref="Id"/>
        /// </summary>
        [JsonProperty("response")]
        public bool IsResponse { get; set; }
        [JsonProperty("type")]
        public string MessageType { get; set; }
        [JsonProperty("message")]
        public byte[] Message { get; set; }
        /// <summary>
        /// If this is true, no more messages with the <see cref="Id"/> should be sent.
        /// </summary>
        [JsonProperty("close")]
        public bool Close { get; set; }
    }
}