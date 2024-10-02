using System.Text.Json.Nodes;
using System.Text.Json;

namespace PiwotOBSDeck.Donations.WebRequests
{
    public class PongRequest : RequestBase
    {
        public PongRequest(string author = "OBSDeck") : base("pong", author)
        {
            Type = RequestType.Pong;
        }
        public override JsonObject ToJson()
        {
            return JsonSerializer.SerializeToNode(this).AsObject();
        }
    }
}
