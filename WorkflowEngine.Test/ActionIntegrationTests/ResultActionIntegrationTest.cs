using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace WorkflowEngine.Test.ActionIntegrationTests
{
    public class ResultActionIntegrationTest
    {
        private readonly ITestOutputHelper testOutputHelper;

        public ResultActionIntegrationTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task ResultAction_ExecuteStrategy_ReturnResult()
        {
            // Arrange
            Workflow workFlow = new Workflow(GetXmlStub(), DC.GetInstance());
            workFlow.CurrentInstance.MaxDegreeOfParallelism = 25;

            // Act
            string res = await workFlow.ExecuteAsync(GetJsonContextDataStub(), CancellationToken.None);
            testOutputHelper.WriteLine(res);

            // Assert
            Assert.Equal(GetExpectedJsonContextDataStub(), res);
        }

        #region Stub
        private static string GetXmlStub()
        {
            StringBuilder xmlStringBuilder = new StringBuilder();
            xmlStringBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            xmlStringBuilder.Append("<Workflow>");
            xmlStringBuilder.Append("  <Start/>");
            xmlStringBuilder.Append("  <Strategy name=\"StrategyDecision\" output=\"StrategyDecision\"/>");
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

        private static string GetExpectedJsonContextDataStub()
        {
            StringBuilder jsonStringBuilder = new StringBuilder();
            jsonStringBuilder.AppendLine("{");
            jsonStringBuilder.AppendLine("  \"key\": \"jkjghjfjfjgfdj\",");
            jsonStringBuilder.AppendLine("  \"value\": \"123\",");
            jsonStringBuilder.AppendLine("  \"Connector_Output\": \"\",");
            jsonStringBuilder.AppendLine("  \"CurrentProcess\": \"StrategyDecision\",");
            jsonStringBuilder.AppendLine("  \"ConnectorBlackList\": {");
            jsonStringBuilder.AppendLine("    \"identificationId\": 810,");
            jsonStringBuilder.AppendLine("    \"clientId\": 20845,");
            jsonStringBuilder.AppendLine("    \"riskLevelId\": 1");
            jsonStringBuilder.AppendLine("  },");
            jsonStringBuilder.AppendLine("  \"SetSuccess\": \"92\",");
            jsonStringBuilder.AppendLine("  \"StartProcess\": \"\",");
            jsonStringBuilder.AppendLine("  \"StrategyDecision\": \"{\\\"BR_01\\\": \\\"92\\\",\\\"BR_02\\\": \\\"0\\\",\\\"BR_03\\\": \\\"901271\\\",\\\"BR_Result\\\": \\\"2\\\",\\\"BR_04\\\": \\\"92\\\",\\\"BR_05\\\": \\\"0\\\",\\\"BR_06\\\": \\\"901271\\\",\\\"BR_052\\\": \\\"92\\\",\\\"BR_Result2\\\": \\\"6\\\"}\"");
            jsonStringBuilder.Append("}");

            return jsonStringBuilder.ToString();
        }

        #endregion
    }
}
