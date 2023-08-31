using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace WorkflowEngine.Test.ActionIntegrationTests
{
    public class PointActionIntegrationTest
    {
        private readonly ITestOutputHelper testOutputHelper;

        public PointActionIntegrationTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task PointAction_GoToPointAndSkipConnectorBlackListAndDataStoreTest()
        {
            // Arrange
            Workflow workFlow = new Workflow(GetXmlStub(), DC.GetInstance());
            workFlow.CurrentInstance.MaxDegreeOfParallelism = 25;

            // Act
            string res = await workFlow.ExecuteAsync(GetJsonContextDataStub(), CancellationToken.None);

            testOutputHelper.WriteLine(res);

            // Assert
            Assert.Equal(GetExpectedJsonStub(), res);
        }

        #region Stub
        private static string GetXmlStub()
        {
            StringBuilder xmlStringBuilder = new StringBuilder();
            xmlStringBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            xmlStringBuilder.Append("<Workflow>");
            xmlStringBuilder.Append("  <Start/>");
            xmlStringBuilder.Append("  <GoTo actionName = \"PointTest\" />");
            xmlStringBuilder.Append("  <Connector name=\"ConnectorBlackList\" output=\"ConnectorBlackList\" saveAs=\"\">");
            xmlStringBuilder.Append("    <parameter type=\"const\" name=\"key\" value=\"keyTest\" />");
            xmlStringBuilder.Append("    <parameter type=\"const\" name=\"value\" value=\"valueTest\" />");
            xmlStringBuilder.Append("  </Connector>");
            xmlStringBuilder.Append("  <DataStore name=\"DataStoreTest\" output=\"DecisionState\" expression=\"{0}+2\">");
            xmlStringBuilder.Append("    <parameter type=\"appData\" value=\"ConnectorBlackList..identificationId\" />");
            xmlStringBuilder.Append("  </DataStore>");
            xmlStringBuilder.Append("  <Point name = \"PointTest\" />");
            xmlStringBuilder.Append("  <Finish/>");
            xmlStringBuilder.Append("</Workflow>");

            return xmlStringBuilder.ToString();
        }

        private static string GetJsonContextDataStub()
        {
            StringBuilder jsonStringBuilder = new StringBuilder();
            jsonStringBuilder.AppendLine("{");
            jsonStringBuilder.AppendLine("  \"key\": \"jkjghjfjfjgfdj\",");
            jsonStringBuilder.AppendLine("  \"value\": \"123\",");
            jsonStringBuilder.AppendLine("  \"Connector_Output\": \"\",");
            jsonStringBuilder.AppendLine($"  \"CurrentProcess\": \"\",");
            jsonStringBuilder.AppendLine("  \"ConnectorBlackList\": {");
            jsonStringBuilder.AppendLine($"    \"identificationId\": 810,");
            jsonStringBuilder.AppendLine("    \"clientId\": 20845,");
            jsonStringBuilder.AppendLine("    \"riskLevelId\": 1");
            jsonStringBuilder.AppendLine("  },");
            jsonStringBuilder.AppendLine("  \"SetSuccess\": \"92\"");
            jsonStringBuilder.Append("}");

            return jsonStringBuilder.ToString();
        }

        private static string GetExpectedJsonStub()
        {
            StringBuilder jsonStringBuilder = new StringBuilder();
            jsonStringBuilder.AppendLine("{");
            jsonStringBuilder.AppendLine("  \"key\": \"jkjghjfjfjgfdj\",");
            jsonStringBuilder.AppendLine("  \"value\": \"123\",");
            jsonStringBuilder.AppendLine("  \"Connector_Output\": \"\",");
            jsonStringBuilder.AppendLine($"  \"CurrentProcess\": \"FinishProcess\",");
            jsonStringBuilder.AppendLine("  \"ConnectorBlackList\": {");
            jsonStringBuilder.AppendLine($"    \"identificationId\": 810,");
            jsonStringBuilder.AppendLine("    \"clientId\": 20845,");
            jsonStringBuilder.AppendLine("    \"riskLevelId\": 1");
            jsonStringBuilder.AppendLine("  },");
            jsonStringBuilder.AppendLine("  \"SetSuccess\": \"92\",");
            jsonStringBuilder.AppendLine("  \"StartProcess\": \"\"");
            jsonStringBuilder.Append("}");

            return jsonStringBuilder.ToString();
        }

        #endregion
    }
}
