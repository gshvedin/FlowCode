using System.Xml.Linq;
using Moq;
using Xunit;
using AutoFixture.Xunit2;
using WorkflowEngine.Core;
using WorkflowEngine.Helpers.Audit;
using WorkflowEngine.Actions.Implementations;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace WorkflowEngine.Test.Action.Implementations
{
    public class GoToActionTest
    {
        #region Constructor
        [Theory, AutoData]
        public void Constructor_CreateInstance_SuccessfullySettingItem(XElement xElement)
        {
            // Arrange

            // Act
            GoToAction goToAction = new GoToAction(xElement);

            // Assert
            Assert.NotEqual(goToAction, default(GoToAction));
            Assert.Equal(xElement, goToAction.Item);
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
            mockContextData.Setup(r => r.GetCurrentRequestId())
                           .Returns(testGuid);
            mockInstance.SetupGet(v => v.AuditItems)
                        .Returns(list);
            mockInstance.Setup(svc => svc.ContextData)
                        .Returns(mockContextData.Object);

            GoToAction goToAction = new GoToAction(item)
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            await goToAction.ExecuteAsync();

            // Assert
            WorkflowAuditItem workflow = list.SingleOrDefault(v => v.NodeName == goToAction.GetType().Name);
            Assert.Equal(string.Empty, workflow.Info);

            mockContextData.Verify(r => r.GetCurrentRequestId());
        }

        [Theory, AutoData]
        internal async Task Execute_SetGoToAction_SuccessfulSet(
                                                   Mock<IInstance> mockInstance,
                                                        Mock<IContextData> mockContextData,
                                                        IList<WorkflowAuditItem> list,
                                                        string testActionNameVal,
                                                        Guid testGuid)
        {
            // Arrange
            mockContextData.Setup(r => r.GetCurrentRequestId())
                            .Returns(testGuid);
            mockInstance.SetupGet(v => v.AuditItems)
                        .Returns(list);
            mockInstance.Setup(svc => svc.ContextData)
                        .Returns(mockContextData.Object);

            XElement item = new XElement("Test", new XAttribute("actionName", testActionNameVal));
            GoToAction goToAction = new GoToAction(item)
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            await goToAction.ExecuteAsync();

            // Assert
            WorkflowAuditItem workflow = list.SingleOrDefault(v => v.NodeName == goToAction.GetType().Name);
            Assert.Equal(string.Empty, workflow.Info);

            mockContextData.Verify(r => r.GetCurrentRequestId());
            mockContextData.VerifySet(m => m.GoToAction = testActionNameVal);
        }
        #endregion
    }
}
