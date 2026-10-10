using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shirase.Pushover.Models
{
    public class Message
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("id_str")]
        public string IdStr { get; set; }

        [JsonPropertyName("message")]
        public string Content { get; set; }

        [JsonPropertyName("app")]
        public string App { get; set; }

        [JsonPropertyName("Aid")]
        public long Aid { get; set; }

        [JsonPropertyName("aid_str")]
        public string AidStr { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        [JsonPropertyName("date")]
        public int Date { get; set; }

        [JsonPropertyName("priority")]
        public int Priority { get; set; }

        [JsonPropertyName("acked")]
        public int Acked { get; set; }

        [JsonPropertyName("umid")]
        public long Umid { get; set; }

        [JsonPropertyName("umid_str")]
        public string UmidStr { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }
    }
}
