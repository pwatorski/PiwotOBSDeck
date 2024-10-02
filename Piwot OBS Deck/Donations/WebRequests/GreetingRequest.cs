using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace PiwotOBSDeck.Donations.WebRequests
{
    public class GreetingRequest : RequestBase
    {


        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("peerType")]
        public string PeerType { get; set; }
        public GreetingRequest(string name, string peerType, string author = "OBSDeck") : base("greeting", author)
        {
            Name = name;
            PeerType = peerType;
            Type = RequestType.Greeting;
        }
        public override JsonObject ToJson()
        {
            return JsonSerializer.SerializeToNode(this).AsObject();
        }
    }
}
