using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using WorkflowEngine.Core;

namespace WorkflowEngine.Helpers
{
    public static class DataExtensions
    {
        public static string GetAttribute(this XElement element, string attributeName, IContextData contextData)
        {
            string value = element?.Attributes().Where(a => a.Name.LocalName == attributeName).FirstOrDefault()?.Value;
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            if (value.StartsWith("#") && contextData != null)
            {
                return contextData.GetArgument(value.Substring(1));
            }

            return value;
        }

        public static string GetHashValue(this XElement element)
        {
            string xml = element.ToString();
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(xml));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString().Substring(0, 8);
            }

        }

    }
}