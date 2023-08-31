using System.Xml.Linq;
using System.Collections.Generic;
using Moq;
using Xunit;
using AutoFixture.Xunit2;
using WorkflowEngine.Actions.Implementations;
using WorkflowEngine.Core;
using WorkflowEngine.Helpers.Audit;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace WorkflowEngine.Test.Action.Implementations
{
    public class PointActionTest
    {
        #region Constructor
        [Theory, AutoData]
        public void Constructor_CreateInstance_SuccessfullySettingItem(XElement xElement)
        {
            // Arrange
            // Act
            PointAction pointAction = new PointAction(xElement);

            // Assert
            Assert.NotEqual(pointAction, default(PointAction));
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

            PointAction pointAction = new PointAction(item)
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
           await pointAction.ExecuteAsync();

            // Assert
            WorkflowAuditItem workflow = list.SingleOrDefault(v => v.NodeName == pointAction.GetType().Name);
            Assert.Equal(string.Empty, workflow.Info);

            mockContextData.Verify(r => r.GetCurrentRequestId());
        }
        #endregion
    }
}
