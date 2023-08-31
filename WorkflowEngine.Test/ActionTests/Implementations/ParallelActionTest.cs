using System;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Moq;
using Xunit;
using AutoFixture.Xunit2;
using WorkflowEngine.Core;
using WorkflowEngine.Helpers.Audit;
using WorkflowEngine.Actions.Implementations;

namespace WorkflowEngine.Test.ActionTests.Implementations
{
    public class ParallelActionTest
    {
        #region Constructor
        [Theory, AutoData]
        public void Constructor_CreateInstance_Successfully(XElement item)
        {
            // Arrange
            // Act
            ParallelAction parallelAction = new ParallelAction(item);

            // Assert
            Assert.NotNull(parallelAction);
            Assert.Equal(item, parallelAction.Item);
        }

        #endregion

        #region Execute
        [Theory, AutoData]
        public async Task Execute_ExecuteAudit_Successful(
                                            Mock<IInstance> mockInstance,
                                            Mock<IContextData> mockContextData,
                                            IList<WorkflowAuditItem> list,
                                            XElement item,
                                            Guid testGuid)
        {
            // Arrange
            mockInstance.SetupGet(p => p.MaxDegreeOfParallelism)
                        .Returns(5);
            mockContextData.Setup(r => r.GetCurrentRequestId())
                           .Returns(testGuid);
            mockInstance.SetupGet(v => v.AuditItems)
                        .Returns(list);
            mockInstance.Setup(svc => svc.ContextData)
                        .Returns(mockContextData.Object);

            ParallelAction parallelAction = new ParallelAction(item)
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            await parallelAction.ExecuteAsync();

            // Assert
            WorkflowAuditItem workflow = list.SingleOrDefault(v => v.NodeName == parallelAction.GetType().Name);
            Assert.Contains("Parallel loop state isCompleted. Execution Time =", workflow.Info, StringComparison.CurrentCulture);

            mockContextData.Verify(r => r.GetCurrentRequestId());
        }
        #endregion

        #region Stub

        private static XElement GetXmlStub()
        {
            XElement xmlTree = XElement.Parse(GetSkipXmlStub());

            return xmlTree;
        }

        private static string GetSkipXmlStub()
        {
            StringBuilder xmlStringBuilder = new StringBuilder();
            xmlStringBuilder.Append("<root>");
            xmlStringBuilder.Append("<UserTask name=\"UserTaskAction\" output=\"UserTaskAction\">");
            xmlStringBuilder.Append("<parameter type=\"appData\" name=\"keyConnector\" value=\"valueConnector\" />");
            xmlStringBuilder.Append("</UserTask>");
            xmlStringBuilder.Append("<Condition name=\"ConditionAction\" output=\"ConditionAction\">");
            xmlStringBuilder.Append("<parameter type=\"appData\" name=\"keyConditionAction\" value=\"valueConditionAction\" />");
            xmlStringBuilder.Append("</Condition>");
            xmlStringBuilder.Append("</root>");

            return xmlStringBuilder.ToString();
        }
        #endregion
    }
}