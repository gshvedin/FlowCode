using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data;

namespace WorkflowEngine.Helpers
{
    public static class JsonExtensions
    {

        public static JObject AddByPath(this JObject obj, string path, JToken newValue)
        {
            if (string.IsNullOrEmpty(path))
            {
                return obj;
            }

            // Normalize the path by removing specific notations used in JSON paths
            if (path.Contains("[0]"))
            {
                // Remove the zero indexer notation to simplify path handling
                path = path.Replace(".[0].", ".").Replace("[0].", ".");
            }
            if (path.Contains("$."))
            {
                // Remove the root object notation
                path = path.Replace("$.", "");
            }

            // Split the path into parts
            string[] slices = path.Split('.');

            // if there no slices just add value, othervise add nested elements recursively
            if (slices.Length == 1)
            {
                if (!string.IsNullOrEmpty(path) && !obj.ContainsKey(path))
                {
                    obj.Add(path, newValue);
                }
            }
            else
            {
                if (!obj.ContainsKey(slices[0]))
                {
                    obj.Add(slices[0], new JObject());
                }
                JObject nestedObj = (JObject)obj[slices[0]];
                nestedObj.AddByPath(path.Replace($"{slices[0]}.", string.Empty), newValue);
            }

            return obj;
        }


        public static JObject AddOrReplaceByPath(this JObject obj, string path, JToken newValue)
        {
            if (string.IsNullOrEmpty(path))
            {
                return obj;
            }

            if (obj.SelectToken(path) is JToken token)
            {
                token.Replace(newValue); // Direct assignment replaces the existing value
                return obj;
            }

            return obj.AddByPath(path, newValue);
        }
    }
}

