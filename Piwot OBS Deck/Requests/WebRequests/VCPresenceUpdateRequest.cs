using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace PiwotOBSDeck.WebServices.WebRequests
{
    public class VCPresenceUpdateRequest : RequestBase
    {


        [JsonPropertyName("storagePath")]
        public string StoragePath { get; set; }

        [JsonPropertyName("timestamp")]
        public double Timestamp { get; set; } = 0;
        [JsonPropertyName("ids")]
        public string[] Ids { get; set; }
        [JsonPropertyName("names")]
        public string[] Names { get; set; }

        [JsonPropertyName("meta")]
        public JsonNode? Meta { get; set; }

        public VCPresenceUpdateRequest(string storagePath, double timestamp, string[] ids, string[] names, JsonNode? meta = null, string author = "OBSDeck") : base("VCPresenceUpdate", author)
        {
            
            StoragePath = storagePath;
            Timestamp = timestamp;
            Ids = ids;
            Names = names;
            Type = RequestType.VCPresence;
            Meta = meta;
        }

        public override JsonObject ToJson()
        {
            return JsonSerializer.SerializeToNode(this).AsObject();
        }
    }
}
