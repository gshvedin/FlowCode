using System.Linq;
using System.Text;
using System.Xml.Linq;
using Moq;
using Xunit;
using AutoFixture.Xunit2;
using WorkflowEngine.Actions;
using WorkflowEngine.Core;
using WorkflowEngine.Actions.Implementations;

namespace WorkflowEngine.Test.CoreTests
{
    public class ActionListTest
    {
        #region Constructor
        [Theory, AutoData]
        public void Constructor_CreateInstanceAndSetSupportWorkflow_SuccessfullySetting(
                                                                     Mock<IInstance> mockInstance,
                                                                     Mock<IWorkflowActionBaseFactory> mockWorkflowAction,
                                                                     Mock<WorkflowActionBase> mockWorkflowActionBase)
        {
            // Arrange
            mockWorkflowAction.Setup(c => c.SupportsActionType(It.IsAny<string>())).Returns(true);
            mockWorkflowAction.Setup(c => c.CreateAction(It.IsAny<XElement>(), mockInstance.Object)).Returns(mockWorkflowActionBase.Object);

            // Act
            ActionList actionList = new ActionList(GetXmlStub(), mockInstance.Object);

            // Assert
            Assert.True(actionList.Any());
        }

        [Theory, AutoData]
        public void Constructor_CreateInstanceWithDefaultAndSetSupportWorkflow_SuccessfullySetting(Mock<IInstance> mockInstance)
        {
            // Arrange
            // Act
            ActionList actionList = new ActionList(GetXmlStub(), mockInstance.Object);

            // Assert
            Assert.True(actionList.Any());
        }

        [Theory, AutoData]
        public void Constructor_CreateInstanceAndSetSupportWorkflow_NotSuccessfullySetting(
                                                                    Mock<IInstance> mockInstance,
                                                                    Mock<IWorkflowActionBaseFactory> mockWorkflowAction,
                                                                    Mock<WorkflowActionBase> mockWorkflowActionBase)
        {
            // Arrange
            mockWorkflowAction.Setup(c => c.SupportsActionType(It.IsAny<string>())).Returns(false);
            mockWorkflowAction.Setup(c => c.CreateAction(It.IsAny<XElement>(), mockInstance.Object)).Returns(mockWorkflowActionBase.Object);

            // Act
            ActionList actionList = new ActionList(GetXmlStub(), mockInstance.Object, mockWorkflowAction.Object);

            // Assert
            Assert.False(actionList.Any());
        }

        #endregion

        #region GetCurrentAction
        [Theory, AutoData]
        public void GetCurrentAction_GoToAction(
                              Mock<IInstance> mockInstance,
                              Mock<IContextData> mockContextData,
                              Mock<IWorkflowActionBaseFactory> mockWorkflowActionBase,
                              XElement xElement)
        {
            // Arrange
            ConnectorAction connectorAction = new ConnectorAction(xElement);

            mockContextData.SetupGet(v => v.GoToAction)
                           .Returns("ConnectorAction");
            mockWorkflowActionBase.Setup(m => m.SupportsActionType(It.IsAny<string>()))
                                  .Returns(true);
            mockWorkflowActionBase.Setup(m => m.CreateAction(It.IsAny<XElement>(), It.IsAny<IInstance>()))
                                  .Returns(connectorAction);

            ActionList actionList = new ActionList(GetXmlStub(), mockInstance.Object, mockWorkflowActionBase.Object);

            // Act
            WorkflowActionBase res = actionList.GetCurrentAction(mockContextData.Object);

            // Assert
            Assert.Equal(connectorAction, res);
        }

        [Theory, AutoData]
        public void GetCurrentAction_GetCurrentProcessByName(
                                                    Mock<IInstance> mockInstance,
                                                    Mock<IContextData> mockContextData,
                                                    Mock<IWorkflowActionBaseFactory> mockWorkflowActionBase,
                                                    XElement xElement)
        {
            // Arrange
            ConnectorAction connectorAction = new ConnectorAction(xElement);
            mockContextData.SetupGet(v => v.GoToAction)
                            .Returns(string.Empty);
            mockContextData.Setup(m => m.GetCurrentProcess())
                           .Returns("ConnectorAction");
            mockWorkflowActionBase.Setup(m => m.SupportsActionType(It.IsAny<string>()))
                                  .Returns(true);
            mockWorkflowActionBase.Setup(m => m.CreateAction(It.IsAny<XElement>(), It.IsAny<IInstance>()))
                                  .Returns(connectorAction);

            ActionList actionList = new ActionList(GetXmlStub(), mockInstance.Object, mockWorkflowActionBase.Object);

            // Act
            WorkflowActionBase res = actionList.GetCurrentAction(mockContextData.Object);

            // Assert
            Assert.Equal(connectorAction, res);
        }

        [Theory, AutoData]
        internal void GetCurrentAction_GetStartProcess(
                                                    Mock<IInstance> mockInstance,
                                                    Mock<IContextData> mockContextData,
                                                    Mock<IWorkflowActionBaseFactory> mockWorkflowActionBase,
                                                    XElement xElement)
        {
            // Arrange
            StartProcess startProcess = new StartProcess(xElement);
            mockContextData.SetupGet(v => v.GoToAction)
                            .Returns(string.Empty);
            mockContextData.Setup(m => m.GetCurrentProcess())
                           .Returns("ConnectorAction");
            mockWorkflowActionBase.Setup(m => m.SupportsActionType(It.IsAny<string>()))
                                  .Returns(true);
            mockWorkflowActionBase.Setup(m => m.CreateAction(It.IsAny<XElement>(), It.IsAny<IInstance>()))
                                  .Returns(startProcess);

            ActionList actionList = new ActionList(GetXmlStub(), mockInstance.Object, mockWorkflowActionBase.Object);

            // Act
            WorkflowActionBase res = actionList.GetCurrentAction(mockContextData.Object);

            // Assert
            Assert.Equal(startProcess, res);
        }

        [Theory, AutoData]
        internal void GetCurrentAction_GetDefaultProcess(
                                                    Mock<IInstance> mockInstance,
                                                    Mock<IContextData> mockContextData,
                                                    Mock<IWorkflowActionBaseFactory> mockWorkflowActionBase,
                                                    XElement xElement)
        {
            // Arrange
            ConditionAction сonditionAction = new ConditionAction(xElement);
            mockContextData.SetupGet(v => v.GoToAction)
                            .Returns(string.Empty);
            mockContextData.Setup(m => m.GetCurrentProcess())
                           .Returns("ConnectorAction");
            mockWorkflowActionBase.Setup(m => m.SupportsActionType(It.IsAny<string>()))
                                  .Returns(true);
            mockWorkflowActionBase.Setup(m => m.CreateAction(It.IsAny<XElement>(), It.IsAny<IInstance>()))
                                  .Returns(сonditionAction);

            ActionList actionList = new ActionList(GetXmlStub(), mockInstance.Object, mockWorkflowActionBase.Object);

            // Act
            WorkflowActionBase res = actionList.GetCurrentAction(mockContextData.Object);

            // Assert
            Assert.Equal(сonditionAction, res);
        }
        #endregion

        #region GetNextAction
        [Theory, AutoData]
        internal void GetNextAction_SetSeveralActions_SuccessfullyGetNext(
                                                     Mock<IInstance> mockInstance,
                                                     Mock<IWorkflowActionBaseFactory> mockWorkflowAction,
                                                     XElement xElement)
        {
            // Arrange
            ConnectorAction connectorAction = new ConnectorAction(xElement);
            StartProcess startProcess = new StartProcess(xElement);
            mockWorkflowAction.Setup(c => c.SupportsActionType(It.IsAny<string>())).Returns(true);
            mockWorkflowAction.SetupSequence(c => c.CreateAction(It.IsAny<XElement>(), It.IsAny<IInstance>()))
                                                   .Returns(connectorAction)
                                                   .Returns(startProcess);

            XElement xElement1 = new XElement("Test1", string.Empty);
            XElement xElement2 = new XElement("Test2", string.Empty);
            XElement xElementTest = new XElement("test", xElement1, xElement2);

            ActionList actionList = new ActionList(xElementTest.ToString(), mockInstance.Object, mockWorkflowAction.Object);

            // Act
            WorkflowActionBase res = actionList.GetNextAction(connectorAction);

            // Assert
            Assert.IsType<StartProcess>(res);
        }

        [Theory, AutoData]
        internal void GetNextAction_GetNextActionAfterLast_ReturnNull(
                                                 Mock<IInstance> mockInstance,
                                                 Mock<IWorkflowActionBaseFactory> mockWorkflowAction,
                                                 XElement xElement)
        {
            // Arrange
            ConnectorAction connectorAction = new ConnectorAction(xElement);
            StartProcess startProcess = new StartProcess(xElement);
            mockWorkflowAction.Setup(c => c.SupportsActionType(It.IsAny<string>())).Returns(true);
            mockWorkflowAction.SetupSequence(c => c.CreateAction(It.IsAny<XElement>(), It.IsAny<IInstance>()))
                                                   .Returns(connectorAction)
                                                   .Returns(startProcess);

            XElement xElement1 = new XElement("Test1", string.Empty);
            XElement xElement2 = new XElement("Test2", string.Empty);
            XElement xElementTest = new XElement("test", xElement1, xElement2);

            ActionList actionList = new ActionList(xElementTest.ToString(), mockInstance.Object, mockWorkflowAction.Object);

            // Act
            WorkflowActionBase res = actionList.GetNextAction(startProcess);

            // Assert
            Assert.Null(res);
        }
        #endregion

        #region Stub
        private static string GetXmlStub()
        {
            StringBuilder xmlStringBuilder = new StringBuilder();
            xmlStringBuilder.Append("<root>");
            xmlStringBuilder.Append("<Connector name=\"ConnectorBlackList\" output=\"ConnectorBlackList\">");
            xmlStringBuilder.Append("<parameter type=\"appData\" name=\"key\" value=\"key\" />");
            xmlStringBuilder.Append("<parameter type=\"appData\" name=\"value\" value=\"value\" />");
            xmlStringBuilder.Append("</Connector>");
            xmlStringBuilder.Append("</root>");

            return xmlStringBuilder.ToString();
        }
        #endregion
    }
}
