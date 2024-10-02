using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace PiwotOBSDeck.Donations.WebRequests
{
    public class RequestBase
    {
        [JsonPropertyName("type")]
        public string TypeString { get; set; }
        [JsonPropertyName("author")]
        public string Author { get; set; }
        [JsonIgnore]
        public RequestType Type { get; set; }
        public enum RequestType { Empty, Greeting, Status, Ping, Pong, Donation, ReciveAcknowledgement }

        public RequestBase(string type = "empty", string author = "OBSDeck")
        {
            TypeString = type;
            Author = author;
            Type = RequestType.Empty;
        }
        public void Send(NetworkStream stream)
        {
            var msgToSend = ToJson().ToJsonString();
            var dataToSend = Encoding.UTF8.GetBytes(msgToSend);
            stream.Write(dataToSend);
        }
        public virtual JsonObject ToJson()
        {
            return JsonSerializer.SerializeToNode(this).AsObject();
        }

        public static List<RequestBase> Parse(string jsonString)
        {
            List<RequestBase> requests = new List<RequestBase>();
            var jsonObj = JsonNode.Parse(jsonString);
            if (jsonObj == null)
            {
                return requests;
            }

            if (jsonObj is JsonArray)
            {
                foreach (var subObj in jsonObj.AsArray())
                {
                    if (subObj == null) continue;
                    RequestBase? result = Parse(subObj.AsObject());
                    if (result == null) continue;
                    requests.Add(result);
                }
            }
            else
            {
                RequestBase? result = Parse(jsonObj.AsObject());
                if (result != null)
                    requests.Add(result);
            }

            return requests;
        }

        public static RequestBase? Parse(JsonObject jsonObject)
        {
            string? type = jsonObject?["type"]?.GetValue<string>();
            if (type == null)
                return null;
            switch (type)
            {
                case "greeting":
                    return jsonObject.Deserialize<GreetingRequest>();
                case "status":
                    return jsonObject.Deserialize<StatusRequest>();
                case "ping":
                    return jsonObject.Deserialize<PingRequest>();
                case "pong":
                    return jsonObject.Deserialize<PongRequest>();
                case "donation":
                    return jsonObject.Deserialize<DonationRequest>();
                case "recivAck":
                    return jsonObject.Deserialize<ReciveAcknowledgementRequest>();
                default: return null;
            }
        }
    }
}
