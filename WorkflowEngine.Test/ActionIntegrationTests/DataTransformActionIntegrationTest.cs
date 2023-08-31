using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WorkflowEngine.Misc;
using Xunit;

namespace WorkflowEngine.Test.ActionIntegrationTests
{
    public class DataTransformActionIntegrationTest
    {
        [Fact]
        public async Task DataTransformAction_ParseConnectorBlackListToXml()
        {
            // Arrange
            Workflow workFlow = new Workflow(GetXmlStub(), DC.GetInstance());
            workFlow.CurrentInstance.MaxDegreeOfParallelism = 25;

            // Act
            string executionResult = await workFlow.ExecuteAsync(GetJsonContextDataStub(), CancellationToken.None);
            JObject jobj = JObject.Parse(executionResult);
            string result = jobj.SelectToken("ConnectorBlackList_OutputTransformed")?.ToString();

            // Assert
            Assert.Equal(
                GetExpectedResult().Replace("\r", string.Empty, StringComparison.CurrentCulture),
                result.Replace("\r", string.Empty, StringComparison.CurrentCulture));
            Assert.Equal("FinishProcess", jobj.SelectToken("CurrentProcess")?.ToString());
        }

        [Fact]
        public async Task DataTransformAction_NotSetTemplateInWorkFlow_ThrowWorkflowException()
        {
            // Arrange
            Workflow workFlow = new Workflow(GetXmlWithoutЕemplateStub(), DC.GetInstance());
            workFlow.CurrentInstance.MaxDegreeOfParallelism = 25;

            // Act
            WorkflowException exception = await Assert.ThrowsAsync<WorkflowException>(() => workFlow.ExecuteAsync(GetJsonContextDataStub(), CancellationToken.None));

            // Assert
            Assert.Equal($"'template' element was not found in action 'ConnectorTransform'", exception.Message);
        }

        #region Stub
        private static string GetExpectedResult()
        {
            StringBuilder xmlStringBuilder = new StringBuilder();
            xmlStringBuilder.Append("<elements>\r\n");
            xmlStringBuilder.Append("  <element name=\"root\">810208451</element>\r\n");
            xmlStringBuilder.Append("  <element name=\"identificationId\">810</element>\r\n");
            xmlStringBuilder.Append("  <element name=\"clientId\">20845</element>\r\n");
            xmlStringBuilder.Append("  <element name=\"riskLevelId\">1</element>\r\n");
            xmlStringBuilder.Append("</elements>");

            return xmlStringBuilder.ToString();
        }

        private static string GetXmlStub()
        {
            StringBuilder xmlStringBuilder = new StringBuilder();
            xmlStringBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            xmlStringBuilder.Append("<Workflow>");
            xmlStringBuilder.Append("  <DataTransform name=\"ConnectorTransform\"");
            xmlStringBuilder.Append("    path=\"ConnectorBlackList\"");
            xmlStringBuilder.Append("    output=\"ConnectorBlackList_OutputTransformed\" >");
            xmlStringBuilder.Append("    <template inputType=\"json\" outputType=\"xml\">");
            xmlStringBuilder.Append("     <![CDATA[");
            xmlStringBuilder.Append("      <xsl:stylesheet xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\"");
            xmlStringBuilder.Append("        xmlns:xs=\"http://www.w3.org/2001/XMLSchema\"");
            xmlStringBuilder.Append("        exclude-result-prefixes=\"xs\"");
            xmlStringBuilder.Append("        version=\"3.0\">");
            xmlStringBuilder.Append("      <xsl:output method=\"xml\" indent=\"yes\" omit-xml-declaration=\"yes\"/>");
            xmlStringBuilder.Append("       <xsl:template match=\" / \">");
            xmlStringBuilder.Append("        <elements>");
            xmlStringBuilder.Append("          <xsl:for-each select=\"//*\">");
            xmlStringBuilder.Append("            <element name=\"{ name(.)}\">");
            xmlStringBuilder.Append("              <xsl:value-of select=\".\" />");
            xmlStringBuilder.Append("            </element>");
            xmlStringBuilder.Append("          </xsl:for-each>");
            xmlStringBuilder.Append("        </elements>");
            xmlStringBuilder.Append("      </xsl:template>");
            xmlStringBuilder.Append("     </xsl:stylesheet>");
            xmlStringBuilder.Append("      ]]>");
            xmlStringBuilder.Append("    </template>");
            xmlStringBuilder.Append("  </DataTransform>");
            xmlStringBuilder.Append("  <Finish/>");
            xmlStringBuilder.Append("</Workflow>");

            return xmlStringBuilder.ToString();
        }

        private static string GetXmlWithoutЕemplateStub()
        {
            StringBuilder xmlStringBuilder = new StringBuilder();
            xmlStringBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            xmlStringBuilder.Append("<Workflow>");
            xmlStringBuilder.Append("  <DataTransform name=\"ConnectorTransform\"");
            xmlStringBuilder.Append("    path=\"ConnectorBlackList\"");
            xmlStringBuilder.Append("    output=\"ConnectorBlackList_OutputTransformed\" >");
            xmlStringBuilder.Append("  </DataTransform>");
            xmlStringBuilder.Append("</Workflow>");

            return xmlStringBuilder.ToString();
        }

        private static string GetJsonContextDataStub()
        {
            StringBuilder jsonStringBuilder = new StringBuilder();
            jsonStringBuilder.Append("{");
            jsonStringBuilder.Append("  \"key\": \"jkjghjfjfjgfdj\",");
            jsonStringBuilder.Append("  \"value\": \"123\",");
            jsonStringBuilder.Append("  \"Connector_Output\": \"\",");
            jsonStringBuilder.Append("  \"CurrentProcess\": \"\",");
            jsonStringBuilder.Append("  \"ConnectorBlackList\": {");
            jsonStringBuilder.Append("    \"identificationId\": 810,");
            jsonStringBuilder.Append("    \"clientId\": 20845,");
            jsonStringBuilder.Append("    \"riskLevelId\": 1");
            jsonStringBuilder.Append("  },");
            jsonStringBuilder.Append("  \"SetSuccess\": \"92\"");
            jsonStringBuilder.Append("}");

            return jsonStringBuilder.ToString();
        }

        #endregion
    }
}
