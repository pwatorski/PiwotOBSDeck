using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace PiwotOBSDeck.Donations.WebRequests
{
    public class ReciveAcknowledgementRequest : RequestBase
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }
        public ReciveAcknowledgementRequest(int count, string author = "OBSDeck") : base("recivAck", author)
        {
            Type = RequestType.ReciveAcknowledgement;
            Count = count;
        }
        public override JsonObject ToJson()
        {
            return JsonSerializer.SerializeToNode(this).AsObject();
        }
    }
}
