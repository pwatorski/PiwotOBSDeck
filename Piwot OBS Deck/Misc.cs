using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace PiwotOBSDeck
{
    internal class Misc
    {
        
        public static JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true
        };

        public static T? TryGetJsonValue<T>(JsonObject jsonObject, string name, T? defaultValue)
        {
            if (jsonObject.TryGetPropertyValue(name, out JsonNode? tempNode))
            {
                if(tempNode == null)
                {
                    return defaultValue;
                }
                return tempNode.GetValue<T?>() ?? defaultValue;
            }
            return defaultValue;
        }
    }
}
