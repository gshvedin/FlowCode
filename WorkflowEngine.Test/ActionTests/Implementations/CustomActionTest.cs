using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Moq;
using Xunit;
using AutoFixture.Xunit2;
using WorkflowEngine.Helpers.Audit;
using WorkflowEngine.Actions.Implementations;
using WorkflowEngine.Core;
using WorkflowEngine.Core.Evaluation;
using WorkflowEngine.Core.Dependencies.CustomActions;
using System;
using System.Threading;
using WorkflowEngine.Misc;

namespace WorkflowEngine.Test.Action.Implementations
{
    public class CustomActionTest
    {
        #region Constructor
        [Theory, AutoData]
        public void Constructor_CreateInstance_SuccessfullySettingItem(XElement xElement)
        {
            // Arrange
            // Act
            CustomAction customAction = new CustomAction(xElement);

            // Assert
            Assert.NotEqual(customAction, default(CustomAction));
            Assert.Equal(xElement, customAction.Item);
        }
        #endregion

        #region Execute
        [Theory, AutoData]
        internal async Task Execute_GetNullCustomAction_ExceptionExpected(
                     string name,
                     Mock<IInstance> mockInstance,
                     Mock<ICustomActionFactory> mockCustomActionFactory)
        {
            // Arrange
            mockCustomActionFactory.Setup(svc => svc.Resolve(It.IsAny<string>()))
                                   .Returns(default(ICustomAction));
            mockInstance.Setup(svc => svc.GetDependency<ICustomActionFactory>())
                        .Returns(mockCustomActionFactory.Object);

            XElement item = new XElement("Test", new XAttribute("name", name));
            CustomAction customAction = new CustomAction(item)
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            WorkflowException exception = await Assert.ThrowsAsync<WorkflowException>(() => customAction.ExecuteAsync());

            // Assert
            Assert.Equal($"CustomAction '{name}' was not resolved.", exception.Message);
            mockCustomActionFactory.Verify(m => m.Resolve(It.Is<string>(v => v == name)));
        }

        [Theory, AutoData]
        internal void Execute_ContextDataSetValue_Successfully(
                                                Mock<IWorkflowAuditService> mockWorkflowAudit,
                                                Mock<IInstance> mockInstance,
                                                Mock<ICustomActionFactory> mockCustomActionFactory,
                                                Mock<ICustomAction> mockCustomAction,
                                                Mock<IContextData> mockContextData,
                                                IList<WorkflowAuditItem> list,
                                                Guid requestId,
                                                string nameTest,
                                                string valueTest)
        {
            // Arrange
            mockInstance.SetupGet(v => v.AuditItems)
                        .Returns(list);
            mockContextData.Setup(r => r.GetCurrentRequestId())
                           .Returns(requestId);
            mockCustomAction.Setup(svc => svc.ExecuteAsync(It.IsAny<Parameters>(), CancellationToken.None))
                            .Returns(Task.FromResult(valueTest));
            mockCustomActionFactory.Setup(svc => svc.Resolve(It.IsAny<string>()))
                                   .Returns(mockCustomAction.Object);
            mockInstance.Setup(svc => svc.GetDependency<ICustomActionFactory>())
                        .Returns(mockCustomActionFactory.Object);
            mockInstance.Setup(svc => svc.GetDependency<IWorkflowAuditService>())
                        .Returns(mockWorkflowAudit.Object);
            mockInstance.Setup(svc => svc.ContextData)
                        .Returns(mockContextData.Object);

            XElement item = new XElement("Test", new XAttribute("output", nameTest));
            CustomAction customAction = new CustomAction(item)
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            customAction.Execute();

            // Assert
            WorkflowAuditItem workflow = list.SingleOrDefault(v => v.NodeName == customAction.GetType().Name);

            mockCustomAction.Verify(m => m.ExecuteAsync(It.IsAny<Parameters>(), CancellationToken.None));
            mockContextData.Verify(m => m.SetValue(It.Is<string>(v => v == nameTest), It.Is<object>(v => v == (object)valueTest)));

            mockInstance.Verify(c => c.GetDependency<ICustomActionFactory>());
            mockCustomActionFactory.Verify(svc => svc.Resolve(It.Is<string>(v => v == customAction.GetType().Name)));
            mockCustomAction.Verify(m => m.ExecuteAsync(It.IsAny<Parameters>(), CancellationToken.None));
            mockContextData.Verify(m => m.SetValue(It.Is<string>(v => v == nameTest), It.Is<string>(v => v == valueTest)));
            mockContextData.Verify(r => r.GetCurrentRequestId());
        }

        [Theory, AutoData]
        internal async Task Execute_SaveAs_Successfully(
             Guid requestId,
             string path1,
             string valueTest,
             Mock<ICustomAction> mockCustomAction,
             Mock<IInstance> mockInstance,
             Mock<IContextData> mockContextData,
             Mock<ICustomActionFactory> mockCustomActionFactory)
        {
            // Arrange
            mockContextData.Setup(r => r.GetCurrentRequestId())
                           .Returns(requestId);
            mockCustomAction.Setup(svc => svc.ExecuteAsync(It.IsAny<Parameters>(), CancellationToken.None))
                            .Returns(Task.FromResult(valueTest));
            mockCustomActionFactory.Setup(svc => svc.Resolve(It.IsAny<string>()))
                                   .Returns(mockCustomAction.Object);
            mockInstance.Setup(svc => svc.GetDependency<ICustomActionFactory>())
                        .Returns(mockCustomActionFactory.Object);
            mockInstance.Setup(svc => svc.ContextData)
                        .Returns(mockContextData.Object);

            XElement item = new XElement("Test", new XAttribute("output", path1), new XAttribute("saveAs", "jn"));
            CustomAction customAction = new CustomAction(item)
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            await customAction.ExecuteAsync();

            // Assert
            mockContextData.Verify(m => m.SetValueAsJsonNode(It.Is<string>(v => v == path1), It.Is<string>(v => v == valueTest)));
        }

        [Theory, AutoData]
        internal async Task Execute_SaveAs_ActionNotFoundException(
             Guid requestId,
             string path1,
             string valueTest,
             Mock<ICustomAction> mockCustomAction,
             Mock<IInstance> mockInstance,
             Mock<IContextData> mockContextData,
             Mock<ICustomActionFactory> mockCustomActionFactory)
        {
            // Arrange
            mockContextData.Setup(r => r.GetCurrentRequestId())
                           .Returns(requestId);
            mockCustomAction.Setup(svc => svc.ExecuteAsync(It.IsAny<Parameters>(), CancellationToken.None))
                            .Returns(Task.FromResult(valueTest));
            mockCustomActionFactory.Setup(svc => svc.Resolve(It.IsAny<string>()))
                                   .Returns((ICustomAction)null);
            mockInstance.Setup(svc => svc.GetDependency<ICustomActionFactory>())
                        .Returns(mockCustomActionFactory.Object);
            mockInstance.Setup(svc => svc.ContextData)
                        .Returns(mockContextData.Object);

            XElement item = new XElement("Test", new XAttribute("output", path1), new XAttribute("saveAs", "jn"));
            CustomAction customAction = new CustomAction(item)
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            WorkflowException exception = await Assert.ThrowsAsync<WorkflowException>(() => customAction.ExecuteAsync());

            // Assert
            Assert.NotNull(exception);
        }

        #endregion
    }
}
