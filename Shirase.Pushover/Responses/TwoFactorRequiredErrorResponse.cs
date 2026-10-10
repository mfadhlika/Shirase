using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shirase.Pushover.Responses
{
    public class TwoFactorRequiredErrorResponse: BaseResponse
    {
        [JsonPropertyName("totp")]
        public string Totp { get; set; }

        [JsonPropertyName("errors")]
        public List<string> Errors { get; set; }
    }
}
