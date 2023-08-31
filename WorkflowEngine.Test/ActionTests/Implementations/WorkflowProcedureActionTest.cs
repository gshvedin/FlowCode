using System;
using System.Xml.Linq;
using Moq;
using Xunit;
using AutoFixture.Xunit2;
using WorkflowEngine.Actions.Implementations;
using WorkflowEngine.Core.WorkflowProcedures;
using WorkflowEngine.Misc;
using System.Threading.Tasks;
using System.Xml;

namespace WorkflowEngine.Test.ActionTests.Implementations
{
    public class WorkflowProcedureActionTest
    {
        #region Constructor
        [Theory, AutoData]
        public void Constructor_CreateInstance_SuccessfullySettingItem(XElement xElement)
        {
            // Arrange, Act
            WorkflowProcedureAction action = new WorkflowProcedureAction(xElement);

            // Assert
            Assert.NotEqual(action, default(WorkflowProcedureAction));
            Assert.Equal(xElement, action.Item);
        }
        #endregion

        #region Execute

        [Theory, AutoData]
        internal async Task Execute_WorkflowContext_Successfully(
             Guid requestId,
             string procedure,
             string valueTest,
             string actionXml,
             Mock<IInstance> mockInstance,
             Mock<IWorkflowProceduresList> mockWorkflowProcedures)
        {
            // Arrange
            mockWorkflowProcedures.Setup(r => r.GetWorkflowProcedure(It.IsAny<string>()))
                           .Returns(actionXml);
            mockInstance.Setup(svc => svc.WorkflowProcedures)
                        .Returns(mockWorkflowProcedures.Object);

            XElement item = new XElement("Test", new XAttribute("procedureName", procedure));
            WorkflowProcedureAction action = new WorkflowProcedureAction(item)
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            XmlException exception = await Assert.ThrowsAsync<XmlException>(() => action.ExecuteAsync());

            // Assert
            Assert.Equal("Data at the root level is invalid. Line 1, position 1.", exception.Message);
            mockWorkflowProcedures.Verify(m => m.GetWorkflowProcedure(It.Is<string>(v => v == procedure)));
        }

        [Theory, AutoData]
        internal async Task Execute_ReturnNullActionXml_ExceptionExpected(
             Guid requestId,
             string procedure,
             string valueTest,
             Mock<IInstance> mockInstance,
             Mock<IWorkflowProceduresList> mockWorkflowProcedures)
        {
            // Arrange
            mockWorkflowProcedures.Setup(r => r.GetWorkflowProcedure(It.IsAny<string>()))
                           .Returns(default(string));
            mockInstance.Setup(svc => svc.WorkflowProcedures)
                           .Returns(mockWorkflowProcedures.Object);

            XElement item = new XElement("Test", new XAttribute("procedureName", procedure));
            WorkflowProcedureAction action = new WorkflowProcedureAction(item)
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            WorkflowException exception = await Assert.ThrowsAsync<WorkflowException>(() => action.ExecuteAsync());

            // Assert
            Assert.Equal($"Workflow procedure '{procedure}' was not resolved", exception.Message);
            mockWorkflowProcedures.Verify(m => m.GetWorkflowProcedure(It.Is<string>(v => v == procedure)));
        }

        #endregion
    }
}
