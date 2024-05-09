using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace WorkflowEngine.Helpers
{
    public static class JsonExtensions
    {

        public static JObject AddByPath(this JObject obj, string path, JToken newValue)
        {

            {
                if (string.IsNullOrEmpty(path))
                {
                    return obj;
                }

                // Remove the root object notation and simplifications for JSONPath
                path = Regex.Replace(path, @"^\$\.|^\$\[|\]$", ""); // Removes "$." at the start and "$[" and "]" at the end
                path = Regex.Replace(path, @"\[0\]", ""); // Simplifies zero indexer in arrays

                // Regex to remove JSONPath query filters like [?(...)]
                path = Regex.Replace(path, @"\[\?\(.*?\)\]", "");

                // Normalize the path to handle relative paths
                path = path.Replace("..", ".");

                // Split the path into parts
                string[] slices = path.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                // If there are no slices or only whitespace, just return the object
                if (slices.Length == 0)
                {
                    return obj;
                }

                // Recursively add nested elements
                if (slices.Length == 1)
                {
                    // Check for an empty slice which might be possible after regex operations
                    if (!string.IsNullOrEmpty(slices[0]) && !obj.ContainsKey(slices[0]))
                    {
                        obj.Add(slices[0], newValue);
                    }
                }
                else
                {
                    if (!obj.ContainsKey(slices[0]))
                    {
                        obj.Add(slices[0], new JObject());
                    }
                    JObject nestedObj = (JObject)obj[slices[0]];
                    // Recursively add the new value after adjusting the path
                    nestedObj.AddByPath(String.Join(".", slices.Skip(1)), newValue);
                }

                return obj;
            }
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

