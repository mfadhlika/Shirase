using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shirase.Pushover.Responses
{
    public class DeviceRegistrationResponse : BaseResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
