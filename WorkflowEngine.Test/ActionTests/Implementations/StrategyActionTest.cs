using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using AutoFixture.Xunit2;
using WorkflowEngine.Helpers.Audit;
using WorkflowEngine.Actions.Implementations;
using WorkflowEngine.Core;
using WorkflowEngine.Core.Dependencies.Strategies;
using WorkflowEngine.Core.Strategies;
using System.Threading;

namespace WorkflowEngine.Test.ActionTests.Implementations
{
    public class StrategyActionTest
    {
        [Theory, AutoData]
        public async Task Execute_SetValue_Successfully(
                                               Mock<IInstance> mockInstance,
                                               Mock<IStrategyService> mockIStrategyService,
                                               Mock<IStrategyContext> mockStrategyContext,
                                               IList<WorkflowAuditItem> list,
                                               Mock<IContextData> mockContextData,
                                               string nameVal,
                                               Guid testGuid)
        {
            // Arrange
            mockStrategyContext.SetupGet(v => v.ContextData)
                               .Returns(GetStrategyContextDataStub());
            mockInstance.SetupGet(v => v.AuditItems)
                        .Returns(list);
            mockContextData.Setup(r => r.GetCurrentRequestId())
                           .Returns(testGuid);
            mockContextData.Setup(m => m.CurrentStrategyContext)
                           .Returns(mockStrategyContext.Object);
            mockInstance.Setup(svc => svc.GetDependency<IStrategyService>())
                        .Returns(mockIStrategyService.Object);
            mockInstance.Setup(svc => svc.ContextData)
                        .Returns(mockContextData.Object);
            mockIStrategyService.Setup(svc => svc.GetStrategyDefinitionAsync(It.IsAny<string>()))
                                .Returns(Task.FromResult(GetXmlFileParsedToString()));

            XElement item = new XElement("Test", new XAttribute("name", nameVal));
            StrategyAction strategyAction = new StrategyAction(item)
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            await strategyAction.ExecuteAsync();

            // Assert
            WorkflowAuditItem workflow = list.SingleOrDefault(v => v.NodeName == nameVal);
            Assert.Equal(GetStrategyContextDataStub().ToJson(), workflow.Info);

            mockContextData.Verify(r => r.GetCurrentRequestId());
            mockStrategyContext.Verify(m => m.ExecuteAsync());
            mockContextData.Verify(m => m.SetValue(It.Is<string>(v => v == nameVal), It.Is<string>(v => v != null)));
            mockInstance.Verify(m => m.GetDependency<IStrategyService>());
            mockIStrategyService.Setup(svc => svc.GetStrategyDefinitionAsync(It.Is<string>(v => v == nameVal)));
        }

        [Theory, AutoData]
        public async Task Execute_SaveAs_Successfully(
                                               Mock<IInstance> mockInstance,
                                               Mock<IStrategyService> mockIStrategyService,
                                               Mock<IStrategyContext> mockStrategyContext,
                                               IList<WorkflowAuditItem> list,
                                               Mock<IContextData> mockContextData,
                                               string nameVal,
                                               Guid testGuid)
        {
            // Arrange
            mockStrategyContext.SetupGet(v => v.ContextData)
                               .Returns(GetStrategyContextDataStub());
            mockInstance.SetupGet(v => v.AuditItems)
                        .Returns(list);
            mockContextData.Setup(r => r.GetCurrentRequestId())
                           .Returns(testGuid);
            mockContextData.Setup(m => m.CurrentStrategyContext)
                           .Returns(mockStrategyContext.Object);
            mockInstance.Setup(svc => svc.GetDependency<IStrategyService>())
                        .Returns(mockIStrategyService.Object);
            mockInstance.Setup(svc => svc.ContextData)
                        .Returns(mockContextData.Object);
            mockIStrategyService.Setup(svc => svc.GetStrategyDefinitionAsync(It.IsAny<string>()))
                                .Returns(Task.FromResult(GetXmlFileParsedToString()));

            XElement item = new XElement("Test", new XAttribute("saveAs", "jn"));
            StrategyAction strategyAction = new StrategyAction(item)
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            await strategyAction.ExecuteAsync();

            // Assert
            mockContextData.Verify(r => r.GetCurrentRequestId());
            mockStrategyContext.Verify(m => m.ExecuteAsync());
            mockContextData.Verify(m => m.SetValueAsJsonNode(It.IsAny<string>(), It.IsAny<string>()));
            mockInstance.Verify(m => m.GetDependency<IStrategyService>());
            mockIStrategyService.Setup(svc => svc.GetStrategyDefinitionAsync(It.Is<string>(v => v == nameVal)));
        }

        #region Stubs
        private static StrategyContextData GetStrategyContextDataStub()
        {
            StrategyContextData strategyContextData = new StrategyContextData
            {
                { "testKey1", "testValue1" },
                { "testKey2", "testValue2" }
            };

            return strategyContextData;
        }

        private static XmlDocument GetXmlStub()
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("Files/Strategy.xml");

            return xDoc;
        }

        private static string GetXmlFileParsedToString()
        {
            XmlDocument xmlDocument = GetXmlStub();
            return xmlDocument.InnerXml;
        }
        #endregion
    }
}
