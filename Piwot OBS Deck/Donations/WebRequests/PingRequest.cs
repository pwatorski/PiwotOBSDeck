using System.Text.Json.Nodes;
using System.Text.Json;

namespace PiwotOBSDeck.Donations.WebRequests
{
    public class PingRequest : RequestBase
    {
        public PingRequest(string author = "OBSDeck") : base("ping", author)
        {
            Type = RequestType.Ping;
        }
        public override JsonObject ToJson()
        {
            return JsonSerializer.SerializeToNode(this).AsObject();
        }
    }
}
