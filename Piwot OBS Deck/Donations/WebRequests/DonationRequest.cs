using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace PiwotOBSDeck.Donations.WebRequests
{
    public class DonationRequest : RequestBase
    {
        [JsonPropertyName("authorName")]
        public string AuthorName { get; set; }
        [JsonPropertyName("text")]
        public string Text { get; set; }
        [JsonPropertyName("value")]
        public double Value { get; set; }
        [JsonPropertyName("id")]
        public long ID { get; set; } = 0;
        [JsonPropertyName("authorId")]
        public long AuthorID { get; set; }
        [JsonPropertyName("meta")]
        public JsonNode? Meta { get; set; }

        public DonationRequest(string authorName, string text, double value, string author = "OBSDeck", long authorID = 0, JsonNode? meta=null) : base("donation", author)
        {
            AuthorName = authorName;
            Text = text;
            Value = value;
            Type = RequestType.Donation;
            AuthorID = authorID;
            Meta = meta;
        }

        public override JsonObject ToJson()
        {
            return JsonSerializer.SerializeToNode(this).AsObject();
        }
    }
}
