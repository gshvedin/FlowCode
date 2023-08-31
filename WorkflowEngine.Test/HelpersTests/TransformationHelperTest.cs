using System;
using System.Text;
using Xunit;
using WorkflowEngine.Helpers;

namespace WorkflowEngine.Test.HelpersTest
{
    public class TransformationHelperTest
    {
        [Fact]
        public void XmlToJson_Transformation_Successfully()
        {
            // Arrange
            // Act
            string res = TransformationHelper.XmlToJson(GetXmlStub());

            // Assert
            Assert.Equal(
                GetJsonStub().Replace("\r", string.Empty, StringComparison.CurrentCulture),
                res.Replace("\r", string.Empty, StringComparison.CurrentCulture));
        }

        [Fact]
        public void JsonToXml_Transformation_Successfully()
        {
            // Arrange
            // Act
            string res = TransformationHelper.JsonToXml(GetJsonStub());

            // Assert
            Assert.Equal(GetXmlStub(), res);
        }

        [Fact]
        public void XsltTransform_TransformationXmlToXslt_Successfully()
        {
            // Arrange
            // Act
            string res = TransformationHelper.XsltTransform(GetXsltStub(), GetXmlStub());

            // Assert
            Assert.Equal(
                TransormatedXslt().Replace("\r", string.Empty, StringComparison.CurrentCulture),
                res.Replace("\r", string.Empty, StringComparison.CurrentCulture));
        }

        #region Stubs
        private static string GetJsonStub()
        {
            StringBuilder jsonStringBuilder = new StringBuilder();
            jsonStringBuilder.Append("{\r\n");
            jsonStringBuilder.Append("  \"Connector\": {\r\n");
            jsonStringBuilder.Append("    \"@name\": \"ConnectorBlackList\",\r\n");
            jsonStringBuilder.Append("    \"@output\": \"ConnectorBlackList\",\r\n");
            jsonStringBuilder.Append("    \"parameter\": [\r\n");
            jsonStringBuilder.Append("      {\r\n");
            jsonStringBuilder.Append("        \"@type\": \"appData\",\r\n");
            jsonStringBuilder.Append("        \"@name\": \"key\",\r\n");
            jsonStringBuilder.Append("        \"@value\": \"key\"\r\n");
            jsonStringBuilder.Append("      },\r\n");
            jsonStringBuilder.Append("      {\r\n");
            jsonStringBuilder.Append("        \"@type\": \"appData\",\r\n");
            jsonStringBuilder.Append("        \"@name\": \"value\",\r\n");
            jsonStringBuilder.Append("        \"@value\": \"value\"\r\n");
            jsonStringBuilder.Append("      }\r\n");
            jsonStringBuilder.Append("    ]\r\n");
            jsonStringBuilder.Append("  }\r\n");
            jsonStringBuilder.Append("}");

            return jsonStringBuilder.ToString();
        }

        private static string GetXmlStub()
        {
            StringBuilder xmlStringBuilder = new StringBuilder();
            xmlStringBuilder.Append("<root>");
            xmlStringBuilder.Append("<Connector name=\"ConnectorBlackList\" output=\"ConnectorBlackList\">");
            xmlStringBuilder.Append("<parameter type=\"appData\" name=\"key\" value=\"key\" />");
            xmlStringBuilder.Append("<parameter type=\"appData\" name=\"value\" value=\"value\" />");
            xmlStringBuilder.Append("</Connector>");
            xmlStringBuilder.Append("</root>");

            return xmlStringBuilder.ToString();
        }

        private static string GetXsltStub()
        {
            StringBuilder xslStringBuilder = new StringBuilder();
            xslStringBuilder.Append(" <xsl:stylesheet xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\"");
            xslStringBuilder.Append("    xmlns:xs=\"http://www.w3.org/2001/XMLSchema\"");
            xslStringBuilder.Append("    exclude-result-prefixes=\"xs\"");
            xslStringBuilder.Append("    version=\"3.0\">");
            xslStringBuilder.Append("  <xsl:output method=\"xml\" indent=\"yes\" omit-xml-declaration=\"yes\"/>");
            xslStringBuilder.Append("   <xsl:template match=\" / \">");
            xslStringBuilder.Append("    <elements>");
            xslStringBuilder.Append("      <xsl:for-each select=\"//*\">");
            xslStringBuilder.Append("        <element name=\"{ name(.)}\">");
            xslStringBuilder.Append("          <xsl:value-of select=\".\" />");
            xslStringBuilder.Append("        </element>");
            xslStringBuilder.Append("      </xsl:for-each>");
            xslStringBuilder.Append("    </elements>");
            xslStringBuilder.Append("  </xsl:template>");
            xslStringBuilder.Append(" </xsl:stylesheet>");

            return xslStringBuilder.ToString();
        }

        private static string TransormatedXslt()
        {
            StringBuilder xmlStringBuilder = new StringBuilder();
            xmlStringBuilder.Append("<elements>\r\n  ");
            xmlStringBuilder.Append("<element name=\"root\"></element>\r\n  ");
            xmlStringBuilder.Append("<element name=\"Connector\"></element>\r\n  ");
            xmlStringBuilder.Append("<element name=\"parameter\"></element>\r\n  ");
            xmlStringBuilder.Append("<element name=\"parameter\"></element>\r\n");
            xmlStringBuilder.Append("</elements>");

            return xmlStringBuilder.ToString();
        }
        #endregion
    }
}