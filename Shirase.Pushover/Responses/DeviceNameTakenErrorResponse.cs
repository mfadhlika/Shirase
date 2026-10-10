using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shirase.Pushover.Responses
{
    public class DeviceNameTakenErrorResponse: BaseResponse
    {
        public class Error
        {
            [JsonPropertyName("name")]
            public List<string> Name { get; set; }
        }

        [JsonPropertyName("errors")]
        public Error Errors { get; set; }
    }
}
