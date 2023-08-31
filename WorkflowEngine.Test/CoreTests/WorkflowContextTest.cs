using System;
using System.Text;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Moq;
using Xunit;
using AutoFixture.Xunit2;
using WorkflowEngine.Core;
using WorkflowEngine.Actions;
using WorkflowEngine.Helpers.Audit;

namespace WorkflowEngine.Test.CoreTests
{
    public class WorkflowContextTest
    {
        #region Constructor
        [Theory, AutoData]
        public void Constructor_CreateInstance_Successfully(Mock<IInstance> mockInstanceWrapper)
        {
            // Arrange
            // Act
            WorkflowContext dataStoreAction = new WorkflowContext(mockInstanceWrapper.Object, GetXmlStub());

            // Assert
            Assert.NotEqual(dataStoreAction, default(WorkflowContext));
            Assert.NotEqual(default(ActionList), dataStoreAction.ActionList);
        }

        [Theory, AutoData]
        public void Constructor_CreateInstance_SuccessfullySettingInstanceWrapper(Mock<IInstance> currentInstance)
        {
            // Arrange
            ActionList actionList = new ActionList(GetXmlStub(), currentInstance.Object);

            // Act
            WorkflowContext dataStoreAction = new WorkflowContext(currentInstance.Object, GetXmlStub())
            {
                ActionList = actionList
            };

            // Assert
            Assert.NotEqual(dataStoreAction, default(WorkflowContext));
            Assert.Equal(actionList, dataStoreAction.ActionList);
        }
        #endregion

        #region Execute
        [Theory, AutoData]
        public async Task Execute_SkipExecute_Successfully(
                                        Mock<IInstance> mockInstance,
                                        Mock<IContextData> mockContextData,
                                        Mock<IWorkflowActionBaseFactory> mockWorkflowActionBase,
                                        XElement item)
        {
            // Arrange
            WorkflowActionTest2 workflowActionTest = new WorkflowActionTest2(item);

            mockWorkflowActionBase.Setup(m => m.SupportsActionType(It.IsAny<string>()))
                               .Returns(true);
            mockWorkflowActionBase.Setup(m => m.CreateAction(It.IsAny<XElement>(), It.IsAny<IInstance>()))
                                  .Returns(workflowActionTest);
            mockContextData.SetupGet(m => m.GoToAction)
                           .Returns(workflowActionTest.GetType().Name);
            mockContextData.Setup(m => m.BreakProcess)
                           .Returns(false);
            mockInstance.Setup(m => m.ContextData)
                        .Returns(mockContextData.Object);

            WorkflowContext workflowContext = new WorkflowContext(mockInstance.Object, GetXmlStub())
            {
                ActionList = new ActionList(GetXmlStub(), mockInstance.Object, mockWorkflowActionBase.Object)
            };

            // Act
            NotImplementedException exception = await Assert.ThrowsAsync<NotImplementedException>(() => workflowContext.ExecuteAsync());

            // Assert
            Assert.Equal("WorkflowActionTest2 error", exception.Message);
            mockContextData.VerifySet(m => m.GoToAction = null);
        }

        [Theory, AutoData]
        public async Task Execute_SkipExecuteAndIsInitialized_Successfully(
                                                         Mock<IInstance> mockInstance,
                                                         Mock<IContextData> mockContextData,
                                                         Mock<IWorkflowActionBaseFactory> mockWorkflowActionBase,
                                                         XElement item)
        {
            // Arrange
            FinishTestProcess finishProcess = new FinishTestProcess(item);

            mockWorkflowActionBase.Setup(m => m.SupportsActionType(It.IsAny<string>()))
                                  .Returns(true);
            mockWorkflowActionBase.Setup(m => m.CreateAction(It.IsAny<XElement>(), It.IsAny<IInstance>()))
                                  .Returns(finishProcess);
            mockContextData.SetupGet(m => m.GoToAction)
                          .Returns(finishProcess.GetType().Name);
            mockContextData.SetupSequence(m => m.BreakProcess)
                               .Returns(false)
                               .Returns(true);
            mockContextData.Setup(m => m.IsInitialized)
                           .Returns(true);
            mockInstance.Setup(m => m.ContextData)
                        .Returns(mockContextData.Object);

            WorkflowContext workflowContext = new WorkflowContext(mockInstance.Object, GetXmlStub())
            {
                ActionList = new ActionList(GetXmlStub(), mockInstance.Object, mockWorkflowActionBase.Object)
            };

            // Act
            NotImplementedException exception = await Assert.ThrowsAsync<NotImplementedException>(() => workflowContext.ExecuteAsync());

            // Assert
            Assert.Equal("FinishTestProcess error", exception.Message);
        }

        [Theory, AutoData]
        public async Task Execute_SkipExecuteAndIsInitializedAndBreakProcess_Successfully(
                                                     Mock<IInstance> mockInstance,
                                                     Mock<IContextData> mockContextData,
                                                     Mock<IWorkflowActionBaseFactory> mockWorkflowActionBase,
                                                     IList<WorkflowAuditItem> list,
                                                     XElement item,
                                                     Guid testGuid)
        {
            // Arrange
            UserTaskAction userTaskAction = new UserTaskAction(item)
            {
                CurrentInstance = mockInstance.Object
            };

            mockWorkflowActionBase.Setup(m => m.SupportsActionType(It.IsAny<string>()))
                                  .Returns(true);
            mockWorkflowActionBase.Setup(m => m.CreateAction(It.IsAny<XElement>(), It.IsAny<IInstance>()))
                                  .Returns(userTaskAction);
            mockContextData.Setup(r => r.GetCurrentRequestId())
                           .Returns(testGuid);
            mockContextData.SetupGet(m => m.GoToAction)
                           .Returns(string.Empty);
            mockContextData.SetupSequence(m => m.BreakProcess)
                           .Returns(false)
                           .Returns(true);
            mockContextData.Setup(m => m.IsInitialized)
                           .Returns(true);
            mockInstance.Setup(m => m.ContextData)
                        .Returns(mockContextData.Object);
            mockInstance.SetupGet(v => v.AuditItems)
                        .Returns(list);

            WorkflowContext workflowContext = new WorkflowContext(mockInstance.Object, GetSkipXmlStub())
            {
                ActionList = new ActionList(GetXmlStub(), mockInstance.Object, mockWorkflowActionBase.Object)
            };

            // Act
            await workflowContext.ExecuteAsync();

            // Assert
            mockContextData.VerifySet(m => m.BreakProcess = true);
            mockContextData.Verify(m => m.SetCurrentProcess(It.Is<WorkflowActionBase>(v => v.Equals(userTaskAction))));
        }

        [Theory, AutoData]
        public async Task Execute_GetNextAction_Successfully(
                                Mock<IInstance> mockInstance,
                                Mock<IContextData> mockContextData)
        {
            // Arrange
            mockContextData.SetupSequence(m => m.BreakProcess)
                           .Returns(false)
                           .Returns(false)
                           .Returns(true);
            mockContextData.Setup(m => m.IsInitialized)
                           .Returns(false);
            mockInstance.Setup(m => m.ContextData)
                        .Returns(mockContextData.Object);

            WorkflowContext workflowContext = new WorkflowContext(mockInstance.Object, GetSkipXmlStub());

            // Act
            await workflowContext.ExecuteAsync();

            // Assert
            mockContextData.Verify(m => m.SetCurrentProcess(It.Is<WorkflowActionBase>(v => v.Name == "ConditionAction")));
        }

        [Theory, AutoData]
        public async Task Execute_Skip_Successfully(
                                Mock<IInstance> mockInstance,
                                Mock<IContextData> mockContextData)
        {
            // Arrange
            mockContextData.SetupSequence(m => m.BreakProcess)
                           .Returns(false)
                           .Returns(true)
                           .Returns(true);
            mockContextData.Setup(m => m.IsInitialized)
                           .Returns(false);
            mockInstance.Setup(m => m.ContextData)
                        .Returns(mockContextData.Object);

            WorkflowContext workflowContext = new WorkflowContext(mockInstance.Object, GetSkipXmlStub());

            // Act
            await workflowContext.ExecuteAsync();

            // Assert
            mockContextData.VerifySet(m => m.GoToAction = It.IsAny<string>(), Times.Never);
            mockContextData.VerifySet(m => m.BreakProcess = It.IsAny<bool>(), Times.Never);
            mockContextData.Verify(m => m.SetCurrentProcess(It.IsAny<WorkflowActionBase>()), Times.Never);
        }
        #endregion

        #region Stub
        private static string GetXmlStub()
        {
            StringBuilder xmlStringBuilder = new StringBuilder();
            xmlStringBuilder.Append("<root>");
            xmlStringBuilder.Append("<Connector name=\"Connector\" output=\"Connector\">");
            xmlStringBuilder.Append("<parameter type=\"appData\" name=\"keyConnector\" value=\"valueConnector\" />");
            xmlStringBuilder.Append("</Connector>");
            xmlStringBuilder.Append("</root>");

            return xmlStringBuilder.ToString();
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

        internal class WorkflowActionTest2 : WorkflowActionBase
        {
            public WorkflowActionTest2(XElement item)
            {
                Item = item;
            }

            public override Task ExecuteAsync()
            {
                throw new NotImplementedException($"{GetType().Name} error");
            }
        }

        internal class FinishTestProcess : WorkflowActionBase
        {
            public FinishTestProcess(XElement item)
            {
                Item = item;
            }

            public override Task ExecuteAsync()
            {
                throw new NotImplementedException($"{GetType().Name} error");
            }
        }

        internal class UserTaskAction : WorkflowActionBase
        {
            public UserTaskAction(XElement item)
            {
                Item = item;
            }

            public override async Task ExecuteAsync()
            {
              await Task.CompletedTask;
            }
        }

        #endregion
    }
}
