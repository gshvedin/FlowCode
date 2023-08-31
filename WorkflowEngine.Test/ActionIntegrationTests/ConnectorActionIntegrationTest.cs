using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xunit;

namespace WorkflowEngine.Test.ActionIntegrationTests
{
    public class ConnectorActionIntegrationTest
    {
        [Fact]
        public async Task ConnectorAction_ExecuteDataconnector()
        {
            // Arrange
            Workflow workFlow = new Workflow(GetXmlStub(), DC.GetInstance());
            workFlow.CurrentInstance.MaxDegreeOfParallelism = 25;

            // Act
            string res = await workFlow.ExecuteAsync(GetJsonContextDataStub(), CancellationToken.None);
            JObject jobj = JObject.Parse(res);

            // Assert
            Assert.Equal("FinishProcess", jobj.SelectToken("CurrentProcess")?.ToString());
            Assert.Equal("{'key': keyTest, 'value': valueTest, 'groupId': -1}", jobj.SelectToken("ConnectorBlackList")?.ToString());
        }

        #region Stub
        private static string GetXmlStub()
        {
            StringBuilder xmlStringBuilder = new StringBuilder();
            xmlStringBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            xmlStringBuilder.Append("<Workflow>");
            xmlStringBuilder.Append("  <Start/>");
            xmlStringBuilder.Append("  <Connector name=\"ConnectorBlackList\" output=\"ConnectorBlackList\" saveAs=\"\">");
            xmlStringBuilder.Append("    <parameter type=\"const\" name=\"key\" value=\"keyTest\" />");
            xmlStringBuilder.Append("    <parameter type=\"const\" name=\"value\" value=\"valueTest\" />");
            xmlStringBuilder.Append("  </Connector>");
            xmlStringBuilder.Append("  <Finish/>");
            xmlStringBuilder.Append("</Workflow>");

            return xmlStringBuilder.ToString();
        }

        private static string GetJsonContextDataStub(string currentProcess = null)
        {
            return "{" +
                   "  \"key\": \"jkjghjfjfjgfdj\"," +
                   "  \"value\": \"123\"," +
                   "  \"Connector_Output\": \"\"," +
                   $"  \"CurrentProcess\": \"{currentProcess}\"," +
                   "  \"ConnectorBlackList\": {" +
                   "    \"identificationId\": 810," +
                   "    \"clientId\": 20845," +
                   "    \"riskLevelId\": 1" +
                   "  }," +
                   "  \"SetSuccess\": \"92\"" +
                   "}";
        }
        #endregion
    }
}
