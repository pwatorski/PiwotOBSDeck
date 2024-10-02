using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace PiwotOBSDeck.Donations.WebRequests
{
    public class StatusRequest : RequestBase
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }
        public StatusRequest(string status, string author = "OBSDeck") : base("status", author)
        {
            Status = status;
            Type = RequestType.Status;
        }
        public override JsonObject ToJson()
        {
            return JsonSerializer.SerializeToNode(this).AsObject();
        }
    }
}
