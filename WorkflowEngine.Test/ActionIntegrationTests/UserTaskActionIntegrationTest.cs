using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xunit;

namespace WorkflowEngine.Test.ActionIntegrationTests
{
    public class UserTaskActionIntegrationTest
    {
        [Fact]
        public async Task ConditionAction_IfFalse_Result92()
        {
            // Arrange
            Workflow workFlow = new Workflow(GetXmlStub(), DC.GetInstance());
            workFlow.CurrentInstance.MaxDegreeOfParallelism = 25;

            // Act
            string res = await workFlow.ExecuteAsync(GetJsonContextDataStub(), CancellationToken.None);
            JObject jobj = JObject.Parse(res);

            // Assert
            Assert.Equal("TestUserTask", jobj.SelectToken("CurrentProcess")?.ToString());
        }

        #region Stub
        private static string GetXmlStub()
        {
            StringBuilder xmlStringBuilder = new StringBuilder();
            xmlStringBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            xmlStringBuilder.Append("<Workflow>");
            xmlStringBuilder.Append("  <Start />");
            xmlStringBuilder.Append("  <UserTask name = \"TestUserTask\"/>");
            xmlStringBuilder.Append("  <Finish />");
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
