using Newtonsoft.Json;
using System.IO;
using System.Xml;
using System.Xml.Xsl;

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
            XmlDocument doc = JsonConvert.DeserializeXmlNode(json, rootName);
            return doc.OuterXml;
        }

        /// <summary>
        /// Трансформация xml файла xslt шаблоном
        /// </summary>
        /// <param name="inputXsltString">входящий xslt файл</param>
        /// <param name="inputXmlString">Xml документ для входа в преобразование</param>
        /// <param name="xslArg">Список аргументов шаблона</param>
        /// <returns>Преобразованный файл в текстовом формате</returns>
        public static string XsltTransform(string inputXsltString, string inputXmlString, XsltArgumentList xslArg = null)
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
            xslt.Transform(inputXml, xslArg ?? new XsltArgumentList(), w);
            w.Close();

            string output = w.ToString();
            return output;
        }
    }
}