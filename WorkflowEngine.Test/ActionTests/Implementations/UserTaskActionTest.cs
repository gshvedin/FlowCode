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

namespace WorkflowEngine.Test.Action.Implementations
{
    public class UserTaskActionTest
    {
        #region Constructor
        [Theory, AutoData]
        public void Constructor_CreateInstance_SuccessfullySettingItem(XElement xElement)
        {
            // Arrange

            // Act
            UserTaskAction userTaskAction = new UserTaskAction(xElement);

            // Assert
            Assert.NotEqual(userTaskAction, default(UserTaskAction));
            Assert.Equal(xElement, userTaskAction.Item);
        }
        #endregion

        #region Execute
        [Theory, AutoData]
        internal async Task Execute_AddAudit_SuccessfulCheckSettingData(
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

            UserTaskAction userTaskAction = new UserTaskAction(item)
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            await userTaskAction.ExecuteAsync();

            // Assert
            WorkflowAuditItem workflow = list.SingleOrDefault(v => v.NodeName == userTaskAction.GetType().Name);
            Assert.Equal(string.Empty, workflow.Info);

            mockContextData.Verify(r => r.GetCurrentRequestId());
            mockContextData.Verify(m => m.SetCurrentProcess(It.Is<UserTaskAction>(v => v.Equals(userTaskAction))));
        }
        #endregion
    }
}
