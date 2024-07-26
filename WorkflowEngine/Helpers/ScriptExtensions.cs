using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowEngine.Helpers
{
    public static class ScriptExtensions
    {
        public static string ExecuteJPathQuery(this string jsonData, string path)
        {
            if (string.IsNullOrEmpty(jsonData) || string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            IEnumerable<JToken> selectedTokens = null;
            if (jsonData.Trim().StartsWith("["))
            {
                var jArray = JArray.Parse(jsonData);
                selectedTokens = jArray.SelectTokens(path);
            }
            else
            {
                JObject data = JObject.Parse(jsonData);
                selectedTokens = data.SelectTokens(path);
            }

            if (selectedTokens == null)
            {
                return string.Empty;
            }
            else if (selectedTokens.Count() == 1)
            {
                return selectedTokens.First().ToString();
            }
            else if (selectedTokens.Count() > 1)
            {
                return selectedTokens.ToString();
            }
            else
            {
                return string.Empty;
            }


        }

        internal static string GenerateGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
