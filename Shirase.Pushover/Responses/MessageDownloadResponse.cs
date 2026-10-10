using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Shirase.Pushover.Models;

namespace Shirase.Pushover.Responses
{
    public class MessageDownloadResponse : BaseResponse
    {
        [JsonPropertyName("messages")]
        public List<Message> Messages { get; set; }

        [JsonPropertyName("user")]
        public User User { get; set; }

        [JsonPropertyName("device")]
        public Device Device { get; set; }
    }
}
