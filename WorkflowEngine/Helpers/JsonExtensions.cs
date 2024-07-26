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

            // Regex to remove JSONPath query filters like [?(...)]
            path = Regex.Replace(path, @"\[\?\(.*?\)\]", "");

            // Normalize the path to handle relative paths
            path = path.Replace("..", ".");
            // Tokenize the path and remove the initial '$' if present
            string[] tokens = path.Split('.');

            JToken current = obj;

            for (int i = 0; i < tokens.Length; i++)
            {
                bool isLastToken = i == tokens.Length - 1;
                string token = tokens[i];
                int nextIndex;

                // Check if current token ends with an index in square brackets e.g., item[0]
                if (Regex.IsMatch(token, @"\[\d+\]$"))
                {
                    string indexPart = Regex.Match(token, @"\[(\d+)\]$").Groups[1].Value;
                    token = Regex.Replace(token, @"\[\d+\]$", ""); // Remove index from token

                    if (current[token] == null)
                    {
                        current[token] = new JArray();
                    }

                    nextIndex = int.Parse(indexPart);
                    while (((JArray)current[token]).Count <= nextIndex)
                    {
                        ((JArray)current[token]).Add(new JObject());
                    }

                    if (isLastToken)
                    {
                        ((JArray)current[token])[nextIndex] = newValue;
                        return obj;
                    }
                    else
                    {
                        current = ((JArray)current[token])[nextIndex];
                    }
                }
                else
                {
                    if (current[token] == null)
                    {
                        current[token] = new JObject();
                    }

                    if (isLastToken)
                    {
                        current[token] = newValue;
                    }
                    else
                    {
                        current = current[token];
                    }
                }
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


        public static JObject AddOrReplaceByPath(this JObject obj, string path, IEnumerable<JToken> tokens)
        {
            if (string.IsNullOrEmpty(path))
            {
                return obj;
            }

            if (obj.SelectToken(path) is JToken token)
            {
                token.Replace(new JArray(tokens));
                return obj;
            }

            return obj.AddByPath(path, new JArray(tokens));
        }


      
    }
}

