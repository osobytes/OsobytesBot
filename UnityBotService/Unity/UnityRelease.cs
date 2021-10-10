using System;
using System.Text;
using Newtonsoft.Json;
using UnityBotService.Serialization;

namespace UnityBotService.Unity
{
    public class UnityRelease
    {
        [JsonConverter(typeof(JsonDateConverter))]
        public virtual DateTime ReleaseDate { get; set; }
        public virtual string Version { get; set; }
        public virtual string UnityVersion { get; set; }

        public new string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Version:\t{Version}");
            builder.AppendLine($"Date:\t{ReleaseDate:d, MMM yyyy}");
            builder.AppendLine($"");
            return builder.ToString();
        }
    }
}