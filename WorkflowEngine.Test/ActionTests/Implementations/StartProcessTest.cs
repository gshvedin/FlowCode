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
    public class StartProcessTest
    {
        #region Constructor
        [Theory, AutoData]
        public void Constructor_CreateInstance_SuccessfullySettingItem(XElement xElement)
        {
            // Arrange

            // Act
            StartProcess pointAction = new StartProcess(xElement);

            // Assert
            Assert.NotEqual(pointAction, default(StartProcess));
            Assert.Equal(xElement, pointAction.Item);
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

            StartProcess startProcess = new StartProcess(item)
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            await startProcess.ExecuteAsync();

            // Assert
            WorkflowAuditItem workflow = list.SingleOrDefault(v => v.NodeName == startProcess.GetType().Name);
            Assert.Equal(string.Empty, workflow.Info);

            mockContextData.Verify(r => r.GetCurrentRequestId());
        }
        #endregion
    }
}
