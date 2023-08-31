using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace WorkflowEngine.Helpers
{
    public static class JsonExtensions
    {
        public static JObject TryAddByPath(this JObject obj, string path, JToken newValue)
        {
            if (string.IsNullOrEmpty(path))
            {
                return obj;
            }

            if (path.Contains("[0]", System.StringComparison.InvariantCulture))
            {
                // escaping zeroindexer
                path = path.Replace(".[0].", ".", System.StringComparison.InvariantCulture).Replace("[0].", ".", System.StringComparison.InvariantCulture);
            }

            // slice path
            string[] slices = path.Split('.');

            // if there no slices just add value, othervise add nested elements recursively
            if (slices.Length == 1)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    obj.TryAdd(path, newValue);
                }
            }
            else
            {
                obj.TryAdd(slices[0], new JObject().TryAddByPath(path.Replace($"{slices[0]}.", string.Empty, System.StringComparison.InvariantCulture), newValue));
            }

            return obj;
        }
    }
}
