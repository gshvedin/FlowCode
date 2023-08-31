using System.Linq;
using System.Xml.Linq;

namespace WorkflowEngine.Helpers
{
    public static class DataExtensions
    {
        public static string GetAttribute(this XElement element, string attributeName)
        {
            return element?.Attributes().Where(a => a.Name.LocalName == attributeName).FirstOrDefault()?.Value;
        }
    }
}