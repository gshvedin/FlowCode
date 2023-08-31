using System;
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
using WorkflowEngine.Core.Strategies;

namespace WorkflowEngine.Test.ActionTests.Implementations
{
    public class SelectCaseActionTest
    {
        #region Execute
        [Theory, AutoData]
        internal async Task Execute_SetNullWorkflowDefinition_ThrowWorkflowException(
                                                   Mock<IInstance> mockInstance,
                                                   Mock<IContextData> mockContextData,
                                                   IList<WorkflowAuditItem> list,
                                                   XElement item,
                                                   Guid testGuid)
        {
            // Arrange
            mockInstance.SetupGet(v => v.AuditItems)
                        .Returns(list);
            mockContextData.Setup(r => r.GetCurrentRequestId())
                            .Returns(testGuid);
            mockInstance.Setup(svc => svc.ContextData)
                        .Returns(mockContextData.Object);

            SelectCaseAction selectCaseAction = new SelectCaseAction(item)
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            await selectCaseAction.ExecuteAsync();

            // Assert
            WorkflowAuditItem workflow = list.SingleOrDefault(v => v.NodeName == selectCaseAction.GetType().Name);
            Assert.Equal("empty execution", workflow.Info);

            mockContextData.Verify(r => r.GetCurrentRequestId());
        }
        #endregion

        #region Execute
        [Theory, AutoData]
        internal async Task Execute_SetDefaultNode_ResultReturned(
                                                   Mock<IInstance> mockInstance,
                                                   Mock<IContextData> mockContextData,
                                                   IList<WorkflowAuditItem> list,
                                                   Guid testGuid)
        {
            // Arrange
            mockInstance.SetupGet(v => v.AuditItems)
                        .Returns(list);
            mockContextData.Setup(r => r.GetCurrentRequestId())
                            .Returns(testGuid);
            mockInstance.Setup(svc => svc.ContextData)
                        .Returns(mockContextData.Object);
            XElement item = new XElement("Test", new XElement("default", "test"));
            SelectCaseAction selectCaseAction = new SelectCaseAction(item)
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            await selectCaseAction.ExecuteAsync();

            // Assert
            mockContextData.Verify(r => r.GetCurrentRequestId());
        }

        [Theory, AutoData]
        internal async Task Execute_SetDefaultContextData_ResultReturned(
                                                   Mock<IInstance> mockInstance,
                                                   Mock<IContextData> mockContextData,
                                                   IList<WorkflowAuditItem> list,
                                                   StrategyContextData data,
                                                   Guid testGuid)
        {
            // Arrange
            mockInstance.SetupGet(v => v.AuditItems)
                        .Returns(list);
            mockContextData.Setup(r => r.GetCurrentRequestId())
                            .Returns(testGuid);
            mockInstance.Setup(svc => svc.ContextData)
                        .Returns(mockContextData.Object);
            XElement item = new XElement("Test", new XElement("default", "test"));
            SelectCaseAction selectCaseAction = new SelectCaseAction(item)
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            await selectCaseAction.ExecuteAsync(data);

            // Assert
            mockContextData.Verify(r => r.GetCurrentRequestId());
        }
        #endregion
    }
}
