using Newtonsoft.Json;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using WorkflowEngine.Core.Dependencies.Counters;
using WorkflowEngine.Core.Dependencies.CustomFunctions;

namespace WorkflowEngine.Helpers
{
    public static class TransformationHelper
    {
        public static string XmlToJson(string xml, Newtonsoft.Json.Formatting formatting = Newtonsoft.Json.Formatting.Indented, bool omitRootElement = true)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return JsonConvert.SerializeXmlNode(doc, formatting, omitRootElement);
        }

        public static string JsonToXml(string json, string rootName = "root")
        {
            bool hasArrayRoot = json.Trim().StartsWith("[");
            // Wrap the JSON array in an object with the specified root element name
            if (hasArrayRoot)
            {
                json = $"{{\"wrapped\": {json}}}";
            }

            // Deserialize the JSON to an XmlDocument
            XmlDocument doc = JsonConvert.DeserializeXmlNode(json, rootName);
            string result = string.Empty;
            if (hasArrayRoot)
            {

                return $"<{rootName}> {doc.SelectSingleNode("root/wrapped").InnerXml}</{rootName}>";
            }
            else
            {
                return doc.InnerXml;
            }
        }

        /// <summary>
        /// Трансформация xml файла xslt шаблоном
        /// </summary>
        /// <param name="inputXsltString">входящий xslt файл</param>
        /// <param name="inputXmlString">Xml документ для входа в преобразование</param>
        /// <param name="xslArg">Список аргументов шаблона</param>
        /// <returns>Преобразованный файл в текстовом формате</returns>
        public static string XsltTransform(string inputXsltString, string inputXmlString, XsltArgumentList xslArg = null, ICustomFunctionProvider functionProvider = null)
        {
            XmlDocument inputXml = new XmlDocument();
            inputXml.LoadXml(inputXmlString);
            XmlReaderSettings xset = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Parse
            };

            // push xslt string to ms
            MemoryStream stream = new MemoryStream();
            using StreamWriter writer = new StreamWriter(stream);
            writer.Write(inputXsltString);
            writer.Flush();
            stream.Position = 0;

            // crate underlying xml reader for xslt
            using XmlReader xread = XmlReader.Create(stream, xset);

            // Create the XslCompiledTransform and load the stylesheet.
            XslCompiledTransform xslt = new XslCompiledTransform();
            XsltSettings settings = new XsltSettings(true, true);
            xslt.Load(xread, settings, new XmlUrlResolver());
            using TextWriter w = new StringWriter();

            xslArg = xslArg ?? new XsltArgumentList();
            if (functionProvider != null)
            {
                xslArg.AddExtensionObject("urn:custom-functions", functionProvider);
            }

            xslt.Transform(inputXml, xslArg, w);
            w.Close();

            string output = w.ToString();
            return output;
        }
    }
}