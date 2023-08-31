using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xunit;

namespace WorkflowEngine.Test.ActionIntegrationTests
{
    public class ConditionActionIntegrationTest
    {
        [Fact]
        public async Task ConditionAction_IfTrue_Result89()
        {
            // Arrange
            Workflow workFlow = new Workflow(GetXmlStub(500), DC.GetInstance());
            workFlow.CurrentInstance.MaxDegreeOfParallelism = 25;

            // Act
            string res = await workFlow.ExecuteAsync(GetJsonContextDataStub(), CancellationToken.None);
            JObject jobj = JObject.Parse(res);

            // Assert
            Assert.Equal("89", jobj.SelectToken("DecisionState")?.ToString());
            Assert.Equal("FinishProcess", jobj.SelectToken("CurrentProcess")?.ToString());
        }

        [Fact]
        public async Task ConditionAction_IfFalse_Result92()
        {
            // Arrange
            Workflow workFlow = new Workflow(GetXmlStub(1000), DC.GetInstance());
            workFlow.CurrentInstance.MaxDegreeOfParallelism = 25;

            // Act
            string res = await workFlow.ExecuteAsync(GetJsonContextDataStub(), CancellationToken.None);

            // Assert
            JObject jobj = JObject.Parse(res);
            Assert.Equal("92", jobj.SelectToken("DecisionState")?.ToString());
            Assert.Equal("FinishProcess", jobj.SelectToken("CurrentProcess")?.ToString());
        }

        #region Stub
        private static string GetXmlStub(int variable)
        {
            return "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
             "<Workflow>" +
             "<Start/>" +
             "<Condition name=\"SelectStrategy\">" +
             "   <test expression=\"{0} >" +
             $"{variable} \">" +
             "      <parameter type=\"appData\" value=\"ConnectorBlackList..identificationId\" />" +
             "    </test>" +
             "    <iftrue>" +
             "      <DataStore name=\"SetFail\" output=\"DecisionState\" expression=\"{0}+2\">" +
             "        <parameter type=\"constant\" value=\"87\" />" +
             "      </DataStore>" +
             "    </iftrue>" +
             "    <iffalse>" +
             "      <DataStore name=\"SetSuccess\" output=\"DecisionState\" expression=\"{0}+2\">" +
             "        <parameter type=\"constant\" value=\"90\" />" +
             "      </DataStore>" +
             "    </iffalse>" +
             "  </Condition>" +
             "  <Finish />" +
             "</Workflow>";
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
