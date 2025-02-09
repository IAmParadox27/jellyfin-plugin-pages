using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.PluginPages.Model
{
    public class PatchRequestPayload
    {
        [JsonPropertyName("contents")]
        public string? Contents { get; set; }
    }
}