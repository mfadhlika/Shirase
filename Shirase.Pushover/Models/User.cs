using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shirase.Pushover.Models
{
    public class User
    {
        [JsonPropertyName("quiet_hours")]
        public bool QuietHours { get; set; }

        [JsonPropertyName("is_android_licensed")]
        public bool IsAndroidLicensed { get; set; }

        [JsonPropertyName("is_ios_licensed")]
        public bool IsIosLicensed { get; set; }

        [JsonPropertyName("is_desktop_licensed")]
        public bool IsDesktopLicensed { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("created_at")]
        public long CreateAt { get; set; }

        [JsonPropertyName("first_email_alias")]
        public string FirstEmailAlias { get; set; }

        [JsonPropertyName("show_tipjar")]
        public string ShowTipjar { get; set; }

        [JsonPropertyName("show_team_ad")]
        public string ShowTeamAd { get; set; }
    }
}
