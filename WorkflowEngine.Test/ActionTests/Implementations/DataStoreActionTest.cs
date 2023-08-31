using System;
using System.Xml.Linq;
using System.Collections.Generic;
using Moq;
using Xunit;
using AutoFixture.Xunit2;
using WorkflowEngine.Core;
using WorkflowEngine.Helpers.Audit;
using WorkflowEngine.Actions.Implementations;
using System.Linq;

namespace WorkflowEngine.Test.Action.Implementations
{
    public class DataStoreActionTest
    {
        #region Constructor
        [Theory, AutoData]
        public void Constructor_CreateInstance_SuccessfullySettingItem(XElement xElement)
        {
            // Arrange
            // Act
            DataStoreAction dataStoreAction = new DataStoreAction(xElement);

            // Assert
            Assert.NotEqual(dataStoreAction, default(DataStoreAction));
            Assert.Equal(xElement, dataStoreAction.Item);
        }
        #endregion

        #region Execute
        [Theory, AutoData]
        public void Execute_AddAudit_SuccessfulCheckSettingData(
                                        Mock<IWorkflowAuditService> mockWorkflowAudit,
                                        Mock<IInstance> mockInstance,
                                        Mock<IContextData> mockContextData,
                                        IList<WorkflowAuditItem> list,
                                        Guid requestId,
                                        string nameVal)
        {
            // Arrange
            mockInstance.SetupGet(v => v.AuditItems)
                        .Returns(list);
            mockContextData.Setup(r => r.GetCurrentRequestId())
                           .Returns(requestId);
            mockInstance.Setup(svc => svc.GetDependency<IWorkflowAuditService>())
                        .Returns(mockWorkflowAudit.Object);
            mockInstance.Setup(svc => svc.ContextData)
                        .Returns(mockContextData.Object);

            XElement item = new XElement("Test", new XAttribute("expression", nameVal), new XAttribute("output", nameVal));
            DataStoreAction dataStoreAction = new DataStoreAction(item)
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            dataStoreAction.Execute();

            // Assert
            WorkflowAuditItem workflow = list.SingleOrDefault(v => v.NodeName == dataStoreAction.GetType().Name);
            Assert.Equal(workflow.Info, string.Empty);

            mockContextData.Verify(m => m.SetValue(It.Is<string>(v => v == nameVal), It.IsAny<string>()));
            mockContextData.Verify(r => r.GetCurrentRequestId());
        }

        [Theory, AutoData]
        public void Execute_SetValueContextData_SetValueFromAtribute(
                                                    Mock<IInstance> mockInstance,
                                                    Mock<IContextData> mockContextData,
                                                    IList<WorkflowAuditItem> list,
                                                    string nameVal)
        {
            // Arrange
            mockInstance.SetupGet(v => v.AuditItems)
                        .Returns(list);
            mockInstance.Setup(svc => svc.ContextData)
                        .Returns(mockContextData.Object);

            XElement item = new XElement("Test", new XAttribute("expression", "test"), new XAttribute("output", nameVal));
            DataStoreAction dataStoreAction = new DataStoreAction(item)
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            dataStoreAction.Execute();

            // Assert
            mockContextData.Verify(m => m.SetValue(It.Is<string>(v => v == nameVal), It.IsAny<string>()));
        }
        #endregion
    }
}
