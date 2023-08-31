using System;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Moq;
using Xunit;
using AutoFixture.Xunit2;
using WorkflowEngine.Misc;
using WorkflowEngine.Core;
using WorkflowEngine.Helpers.Audit;
using WorkflowEngine.Actions.Implementations;
using WorkflowEngine.Core.Dependencies.Connectors;
using WorkflowEngine.Core.Evaluation;
using WorkflowEngine.Core.Dependencies;
using System.Threading;

namespace WorkflowEngine.Test.ActionTests.Implementations
{
    public class ConnectorActionTest
    {
        #region Constructor
        [Theory, AutoData]
        public void Constructor_CreateInstance_SuccessfullySettingItem(XElement xElement)
        {
            // Arrange

            // Act
            ConnectorAction connectorAction = new ConnectorAction(xElement);

            // Assert
            Assert.NotEqual(connectorAction, default(ConnectorAction));
            Assert.Equal(xElement, connectorAction.Item);
        }
        #endregion

        #region Execute

        [Theory, AutoData]
        public async Task Execute_GetNullConnector_ThrowWorkflowException(
                                               Mock<IInstance> mockInstance,
                                               Mock<IConnectorFactory> mockConnectorFactory,
                                               string nameVal)
        {
            // Arrange
            mockInstance.Setup(svc => svc.GetDependency<IConnectorFactory>())
                               .Returns(mockConnectorFactory.Object);
            mockConnectorFactory.Setup(svc => svc.Resolve(It.IsAny<string>()))
                                .Returns((IConnector)null);

            XElement item = new XElement("Test", new XAttribute("name", nameVal));
            ConnectorAction connectorAction = new ConnectorAction(item)
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            WorkflowException exception = await Assert.ThrowsAsync<WorkflowException>(() => connectorAction.ExecuteAsync());

            // Assert
            Assert.Equal($"Connector '{nameVal}' was not resolved.", exception.Message);
            mockInstance.Verify(m => m.GetDependency<IConnectorFactory>());
            mockConnectorFactory.Verify(m => m.Resolve(It.Is<string>(v => v == nameVal)));
        }

        [Theory, AutoData]
        public void Execute_SetValue_Successfully(
                                      Mock<IInstance> mockInstance,
                                      Mock<IDependencyContainer> mockDependencyContainer,
                                      Mock<IConnectorFactory> mockConnectorFactory,
                                      Mock<IConnector> mockConnector,
                                      Mock<IContextData> mockContextData,
                                      Dictionary<string, object> dictionaryTest,
                                      IList<WorkflowAuditItem> list,
                                      string nameVal,
                                      string outputVal,
                                      string resultVal)
        {
            // Arrange
            mockInstance.SetupGet(v => v.AuditItems)
                        .Returns(list);
            mockInstance.Setup(svc => svc.GetDependency<IConnectorFactory>())
                        .Returns(mockConnectorFactory.Object);
            mockInstance.Setup(svc => svc.ContextData)
                        .Returns(mockContextData.Object);
            mockInstance.SetupGet(v => v.DC)
                        .Returns(mockDependencyContainer.Object);
            mockDependencyContainer.SetupGet(v => v.MetaInfo)
                                   .Returns(dictionaryTest);
            mockConnectorFactory.Setup(svc => svc.Resolve(It.IsAny<string>()))
                                .Returns(mockConnector.Object);
            mockConnector.Setup(m => m.ExecuteAsync(It.IsAny<Parameters>(), CancellationToken.None))
                         .Returns(Task.FromResult(resultVal));

            XElement item = new XElement("Test", new XAttribute("name", nameVal), new XAttribute("output", outputVal));
            ConnectorAction connectorAction = new ConnectorAction(item)
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            connectorAction.Execute();

            // Assert
            mockInstance.Verify(m => m.GetDependency<IConnectorFactory>());
            mockConnectorFactory.Verify(m => m.Resolve(It.Is<string>(v => v == nameVal)));
            mockContextData.Verify(m => m.SetValue(It.Is<string>(v => v == outputVal), It.Is<string>(v => v == resultVal)));
        }

        [Theory, AutoData]
        public void Execute_SetValueAsJsonNode_Successfully(
                                                    Mock<IInstance> mockInstance,
                                                    Mock<IDependencyContainer> mockDependencyContainer,
                                                    Mock<IConnectorFactory> mockConnectorFactory,
                                                    Mock<IConnector> mockConnector,
                                                    Mock<IContextData> mockContextData,
                                                    Dictionary<string, object> dictionaryTest,
                                                    IList<WorkflowAuditItem> list,
                                                    string nameVal,
                                                    string outputVal,
                                                    string resultVal)
        {
            // Arrange
            mockInstance.SetupGet(v => v.AuditItems)
                        .Returns(list);
            mockInstance.Setup(svc => svc.GetDependency<IConnectorFactory>())
                        .Returns(mockConnectorFactory.Object);
            mockInstance.Setup(svc => svc.ContextData)
                        .Returns(mockContextData.Object);
            mockInstance.SetupGet(v => v.DC)
                        .Returns(mockDependencyContainer.Object);
            mockDependencyContainer.SetupGet(v => v.MetaInfo)
                                   .Returns(dictionaryTest);
            mockConnectorFactory.Setup(svc => svc.Resolve(It.IsAny<string>()))
                                .Returns(mockConnector.Object);
            mockConnector.Setup(m => m.ExecuteAsync(It.IsAny<Parameters>(), CancellationToken.None))
                         .Returns(Task.FromResult(resultVal));

            string saveAsVal = "jnTest";
            XElement item = new XElement("Test", new XAttribute("name", nameVal), new XAttribute("output", outputVal), new XAttribute("saveAs", saveAsVal));
            ConnectorAction connectorAction = new ConnectorAction(item)
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            connectorAction.Execute();

            // Assert
            mockContextData.Verify(m => m.SetValueAsJsonNode(It.Is<string>(v => v == outputVal), It.Is<string>(v => v == resultVal)));
            mockInstance.Verify(m => m.GetDependency<IConnectorFactory>());
            mockConnectorFactory.Verify(m => m.Resolve(It.Is<string>(v => v == nameVal)));
        }

        [Theory, AutoData]
        public void Execute_ExecuteAudit_Successfully(
                                           Mock<IInstance> mockInstance,
                                           Mock<IDependencyContainer> mockDependencyContainer,
                                           Mock<IWorkflowAuditService> mockWorkflowAudit,
                                           Mock<IConnectorFactory> mockConnectorFactory,
                                           Mock<IConnector> mockConnector,
                                           Mock<IContextData> mockContextData,
                                           Dictionary<string, object> dictionaryTest,
                                           IList<WorkflowAuditItem> list,
                                           Guid requestId,
                                           string nameVal,
                                           string outputVal,
                                           string resultVal)
        {
            // Arrange
            mockInstance.SetupGet(v => v.AuditItems)
                        .Returns(list);
            mockContextData.Setup(r => r.GetCurrentRequestId())
                           .Returns(requestId);
            mockInstance.Setup(svc => svc.GetDependency<IConnectorFactory>())
                        .Returns(mockConnectorFactory.Object);
            mockInstance.Setup(svc => svc.GetDependency<IWorkflowAuditService>())
                        .Returns(mockWorkflowAudit.Object);
            mockInstance.Setup(svc => svc.ContextData)
                        .Returns(mockContextData.Object);
            mockInstance.SetupGet(v => v.DC)
                        .Returns(mockDependencyContainer.Object);
            mockDependencyContainer.SetupGet(v => v.MetaInfo)
                                   .Returns(dictionaryTest);
            mockConnectorFactory.Setup(svc => svc.Resolve(It.IsAny<string>()))
                        .Returns(mockConnector.Object);
            mockConnector.Setup(m => m.ExecuteAsync(It.IsAny<Parameters>(), CancellationToken.None))
                         .Returns(Task.FromResult(resultVal));

            XElement item = new XElement("Test", new XAttribute("name", nameVal), new XAttribute("output", outputVal));
            ConnectorAction connectorAction = new ConnectorAction(item)
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            connectorAction.Execute();

            // Assert
            WorkflowAuditItem workflow = list.SingleOrDefault(v => v.NodeName == nameVal);
            Assert.Contains("Execution duration", workflow.Info, StringComparison.CurrentCulture);

            mockInstance.Verify(c => c.GetDependency<IConnectorFactory>());
            mockConnectorFactory.Verify(svc => svc.Resolve(It.Is<string>(v => v == nameVal)));
            mockConnector.Verify(m => m.ExecuteAsync(It.IsAny<Parameters>(), CancellationToken.None));
            mockContextData.Verify(m => m.SetValue(It.Is<string>(v => v == outputVal), It.Is<string>(v => v == resultVal)));
            mockContextData.Verify(r => r.GetCurrentRequestId());
        }
        #endregion
    }
}
