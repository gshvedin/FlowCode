using System;
using System.Text;
using System.Globalization;
using Xunit;
using WorkflowEngine.Helpers.Audit;

namespace WorkflowEngine.Test.HelpersTests.AuditTest
{
    public class WorkflowAuditItemTest
    {
        [Fact]
        internal void ToString_SerializeObject_Successfully()
        {
            // Arrange
            WorkflowAuditItem workflowAuditItem = GetWorkflowAuditItemStub();

            // Act
            string res = workflowAuditItem.ToString();

            // Assert
            Assert.Equal(GetJsonStub(), res);
        }

        #region Stubs
        private static WorkflowAuditItem GetWorkflowAuditItemStub()
        {
            return new WorkflowAuditItem
            {
                AuditId = 1,
                RequestId = Guid.Parse("6F9619FF-8B86-D011-B42D-00CF4FC964FF"),
                NodeId = Guid.Parse("1A3B944E-3632-467B-A53A-206305310BAE"),
                NodeName = "TestName",
                Timestamp = DateTime.ParseExact("2021-09-23T11:35:09", "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture),
                Info = "InfoTest",
                State = WorkflowAuditState.Success
            };
        }

        private static string GetJsonStub()
        {
            StringBuilder jsonStringBuilder = new StringBuilder();
            jsonStringBuilder.Append("{");
            jsonStringBuilder.Append("\"AuditId\":1,");
            jsonStringBuilder.Append("\"RequestId\":\"6f9619ff-8b86-d011-b42d-00cf4fc964ff\",");
            jsonStringBuilder.Append("\"NodeId\":\"1a3b944e-3632-467b-a53a-206305310bae\",");
            jsonStringBuilder.Append("\"NodeName\":\"TestName\",");
            jsonStringBuilder.Append("\"Timestamp\":\"2021-09-23T11:35:09\",");
            jsonStringBuilder.Append("\"Info\":\"InfoTest\",");
            jsonStringBuilder.Append("\"State\":1");
            jsonStringBuilder.Append("}");

            return jsonStringBuilder.ToString();
        }

        #endregion
    }
}
