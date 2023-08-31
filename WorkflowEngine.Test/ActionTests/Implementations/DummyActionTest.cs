using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Moq;
using Xunit;
using AutoFixture.Xunit2;
using WorkflowEngine.Core;
using WorkflowEngine.Helpers.Audit;
using WorkflowEngine.Actions.Implementations;

namespace WorkflowEngine.Test.Action.Implementations
{
    public class DummyActionTest
    {
        #region Execute
        [Theory, AutoData]
        internal async Task Execute_AddAudit_SuccessfulCheckSettingData(
                                                    Mock<IInstance> mockInstance,
                                                    Mock<IContextData> mockContextData,
                                                    IList<WorkflowAuditItem> list,
                                                    Guid testGuid)
        {
            // Arrange
            mockContextData.Setup(r => r.GetCurrentRequestId())
                          .Returns(testGuid);
            mockInstance.SetupGet(v => v.AuditItems)
                        .Returns(list);
            mockInstance.Setup(svc => svc.ContextData)
                        .Returns(mockContextData.Object);

            DummyAction dummyAction = new DummyAction()
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            await dummyAction.ExecuteAsync();

            // Assert
            WorkflowAuditItem workflow = list.SingleOrDefault(v => v.NodeName == dummyAction.GetType().Name);
            Assert.Equal(string.Empty, workflow.Info);

            mockContextData.Verify(r => r.GetCurrentRequestId());
        }
        #endregion
    }
}