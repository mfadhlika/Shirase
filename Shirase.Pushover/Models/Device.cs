using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shirase.Pushover.Models
{
    public class Device
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("encryption_enabled")]
        public bool EncryptionEnabled { get; set; }

        [JsonPropertyName("default_sound")]
        public string DefaultSound { get; set; }

        [JsonPropertyName("always_use_default_sound")]
        public bool AlwaysUseDefaultSound { get; set; }

        [JsonPropertyName("default_high_priority_sound")]
        public string DefaultHighPrioritySound { get; set; }

        [JsonPropertyName("always_use_default_high_priority_sound")]
        public bool AlwaysUseDefaultHighPrioritySound { get; set; }

        [JsonPropertyName("dismissal_sync_enabled")]
        public bool DismissalSyncEnabled { get; set; }
    }
}
