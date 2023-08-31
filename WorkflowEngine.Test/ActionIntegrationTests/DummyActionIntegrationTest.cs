using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace WorkflowEngine.Test.ActionIntegrationTests
{
    public class DummyActionIntegrationTest
    {
        [Fact]
        public async Task DummyAction_DoNothing()
        {
            // Arrange
            Workflow workFlow = new Workflow(GetXmlStub(), DC.GetInstance());
            workFlow.CurrentInstance.MaxDegreeOfParallelism = 25;

            // Act
            string res = await workFlow.ExecuteAsync(GetJsonContextDataStub(), CancellationToken.None);

            // Assert
            Assert.Equal(GetJsonContextDataStub("FinishProcess"), res.Replace("\r", string.Empty, StringComparison.InvariantCulture));
        }

        #region Stub
        private static string GetXmlStub()
        {
            StringBuilder xmlStringBuilder = new StringBuilder();
            xmlStringBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            xmlStringBuilder.Append("<Workflow>");
            xmlStringBuilder.Append("  <Start/>");
            xmlStringBuilder.Append("  <Dummy/>");
            xmlStringBuilder.Append("  <Finish/>");
            xmlStringBuilder.Append("</Workflow>");

            return xmlStringBuilder.ToString();
        }

        private static string GetJsonContextDataStub(string currentProcess = null)
        {
            return "{\n" +
                    "  \"key\": \"jkjghjfjfjgfdj\",\n" +
                    "  \"value\": \"123\",\n" +
                    "  \"Connector_Output\": \"\",\n" +
                    $"  \"CurrentProcess\": \"{currentProcess}\",\n" +
                    "  \"ConnectorBlackList\": {\n" +
                    "    \"identificationId\": 810,\n" +
                    "    \"clientId\": 20845,\n" +
                    "    \"riskLevelId\": 1\n" +
                    "  },\n" +
                    "  \"SetSuccess\": \"92\",\n" +
                     "  \"StartProcess\": \"\"\n" +
                    "}";
        }
        #endregion
    }
}
