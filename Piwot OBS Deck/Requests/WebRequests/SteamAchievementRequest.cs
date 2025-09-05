using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace PiwotOBSDeck.WebServices.WebRequests
{
    public class SteamAchievementRequest : RequestBase
    {


        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; } = "";
        [JsonPropertyName("storagePath")]
        public string StoragePath { get; set; }
        [JsonPropertyName("backgroundPath")]
        public string BackgroundPath { get; set; }
        [JsonPropertyName("iconPath")]
        public string IconPath { get; set; }
        
        [JsonPropertyName("id")]
        public long ID { get; set; } = 0;

        [JsonPropertyName("meta")]
        public JsonNode? Meta { get; set; }

        public SteamAchievementRequest(string name, string? description, string storagePath, string backgroundPath, string iconPath, JsonNode? meta=null) : base("donation", "OBSDeck")
        {
            Name = name;
            Description = description;
            StoragePath = storagePath;
            BackgroundPath = backgroundPath;
            IconPath = iconPath;
            Type = RequestType.SteamAchievement;
            Meta = meta;
        }

        public override JsonObject ToJson()
        {
            return JsonSerializer.SerializeToNode(this).AsObject();
        }
    }
}
