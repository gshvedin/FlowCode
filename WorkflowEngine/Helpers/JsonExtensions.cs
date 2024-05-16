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
            if (string.IsNullOrEmpty(path))
            {
                return obj;
            }

            // Remove the root object notation and simplifications for JSONPath
            path = Regex.Replace(path, @"^\$\.|^\$\[|\]$", ""); // Removes "$." at the start and "$[" and "]" at the end
            //path = Regex.Replace(path, @"\[0\]", ""); // Simplifies zero indexer in arrays

            // Regex to remove JSONPath query filters like [?(...)]
            path = Regex.Replace(path, @"\[\?\(.*?\)\]", "");

            // Normalize the path to handle relative paths
            path = path.Replace("..", ".");
            // Tokenize the path and remove the initial '$' if present
            string[] tokens = path.Split('.');
            JToken current = obj;

            for (int i = 0; i < tokens.Length - 1; i++)
            {
                // If the current segment does not exist, create it
                if (current[tokens[i]] == null)
                {
                    // If the next segment is numeric, assume an array is needed
                    if (i + 1 < tokens.Length && tokens[i + 1].All(char.IsDigit))
                        current[tokens[i]] = new JArray();
                    else
                        current[tokens[i]] = new JObject();
                }
                // Move to the next token in the path
                current = current[tokens[i]];
            }

            // Set the new value at the final token in the path
            current[tokens.Last()] = newValue;

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

