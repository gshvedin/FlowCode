using System;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using AutoFixture.Xunit2;
using WorkflowEngine.Core;
using WorkflowEngine.Misc;
using WorkflowEngine.Helpers.Audit;
using WorkflowEngine.Actions.Implementations;

namespace WorkflowEngine.Test.ActionTests.Implementations
{
    public class DataTransformActionTest
    {
        [Theory, AutoData]
        internal async Task Execute_NotSetTemplateInItem_ThrowWorkflowException(
                                                     Mock<IInstance> mockInstance,
                                                     string nameVal)
        {
            // Arrange
            XElement item = new XElement("Test", new XAttribute("name", nameVal));
            DataTransformAction dataTransformAction = new DataTransformAction(item)
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            WorkflowException exception = await Assert.ThrowsAsync<WorkflowException>(() => dataTransformAction.ExecuteAsync());

            // Assert
            Assert.Equal($"'template' element was not found in action '{nameVal}'", exception.Message);
        }

        [Theory, AutoData]
        internal async Task Execute_SetValueAsJsonNodeInContextData_Successfully(
                                                             Mock<IInstance> mockInstance,
                                                             Mock<IContextData> mockContextData,
                                                             IList<WorkflowAuditItem> list,
                                                             string outputVal)
        {
            // Arrange
            mockInstance.SetupGet(v => v.AuditItems)
                        .Returns(list);
            mockContextData.Setup(m => m.GetValue(It.IsAny<string>()))
                           .Returns(GetJsonStub());
            mockInstance.Setup(svc => svc.ContextData)
                        .Returns(mockContextData.Object);

            XElement template = new XElement("template", new XAttribute("inputType", "json"), new XAttribute("outputType", "json"));
            template.SetValue(GetXsltStub());

            XElement item = new XElement("Root", new XAttribute("path", GetJsonStub()), new XAttribute("output", outputVal));
            item.Add(template);

            DataTransformAction dataTransformAction = new DataTransformAction(item)
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            await dataTransformAction.ExecuteAsync();

            // Assert
            mockContextData.Verify(m => m.GetValue(It.Is<string>(v => v.Replace("\r", string.Empty, StringComparison.CurrentCulture) ==
                                                                      GetJsonStub().Replace("\r", string.Empty, StringComparison.CurrentCulture))));
            mockContextData.Verify(m => m.SetValueAsJsonNode(
                It.Is<string>(v => v == outputVal),
                It.Is<string>(v => v.Replace("\r", string.Empty, StringComparison.CurrentCulture) ==
                                   GetJsonResultStub().Replace("\r", string.Empty, StringComparison.CurrentCulture))));
        }

        [Theory, AutoData]
        internal async Task Execute_SetValueInContextData_Successfully(
                                                 Mock<IInstance> mockInstance,
                                                Mock<IContextData> mockContextData,
                                                 IList<WorkflowAuditItem> list,
                                                 Guid requestId,
                                                 string outputVal)
        {
            // Arrange
            mockInstance.SetupGet(v => v.AuditItems)
                        .Returns(list);
            mockContextData.Setup(r => r.GetCurrentRequestId())
                           .Returns(requestId);
            mockContextData.Setup(m => m.GetValue(It.IsAny<string>()))
                           .Returns(GetJsonStub());
            mockInstance.Setup(svc => svc.ContextData)
                        .Returns(mockContextData.Object);

            XElement template = new XElement("template", new XAttribute("inputType", "json"));
            template.SetValue(GetXsltStub());

            XElement item = new XElement("Root", new XAttribute("path", GetJsonStub()), new XAttribute("output", outputVal));
            item.Add(template);

            DataTransformAction dataTransformAction = new DataTransformAction(item)
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            await dataTransformAction.ExecuteAsync();

            // Assert
            WorkflowAuditItem workflow = list.SingleOrDefault(v => v.NodeName == dataTransformAction.GetType().Name);
            Assert.Equal(string.Empty, workflow.Info);

            mockContextData.Verify(m => m.GetValue(It.Is<string>(v => v.Replace("\r", string.Empty, StringComparison.CurrentCulture) ==
                                                                      GetJsonStub().Replace("\r", string.Empty, StringComparison.CurrentCulture))));
            mockContextData.Verify(m => m.SetValue(
                                      It.Is<string>(v => v == outputVal),
                                      It.Is<string>(v => v.Replace("\r", string.Empty, StringComparison.CurrentCulture) ==
                                                         GetXmlResultStub().Replace("\r", string.Empty, StringComparison.CurrentCulture))));
            mockContextData.Verify(r => r.GetCurrentRequestId());
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

        private static string GetJsonResultStub()
        {
            StringBuilder xslStringBuilder = new StringBuilder();
            xslStringBuilder.Append("{\r\n");
            xslStringBuilder.Append("  \"element\": [\r\n");
            xslStringBuilder.Append("    {\r\n");
            xslStringBuilder.Append("      \"@name\": \"root\"\r\n");
            xslStringBuilder.Append("    },\r\n");
            xslStringBuilder.Append("    {\r\n");
            xslStringBuilder.Append("      \"@name\": \"Connector\"\r\n");
            xslStringBuilder.Append("    },\r\n");
            xslStringBuilder.Append("    {\r\n");
            xslStringBuilder.Append("      \"@name\": \"parameter\"\r\n");
            xslStringBuilder.Append("    },\r\n");
            xslStringBuilder.Append("    {\r\n");
            xslStringBuilder.Append("      \"@name\": \"parameter\"\r\n");
            xslStringBuilder.Append("    }\r\n");
            xslStringBuilder.Append("  ]\r\n}");

            return xslStringBuilder.ToString();
        }

        private static string GetXmlResultStub()
        {
            StringBuilder xslStringBuilder = new StringBuilder();
            xslStringBuilder.Append("<elements>\r\n");
            xslStringBuilder.Append("  <element name=\"root\"></element>\r\n");
            xslStringBuilder.Append("  <element name=\"Connector\"></element>\r\n");
            xslStringBuilder.Append("  <element name=\"parameter\"></element>\r\n");
            xslStringBuilder.Append("  <element name=\"parameter\"></element>\r\n");
            xslStringBuilder.Append("</elements>");

            return xslStringBuilder.ToString();
        }

        #endregion
    }
}
