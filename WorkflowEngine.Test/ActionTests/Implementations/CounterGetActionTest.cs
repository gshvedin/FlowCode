using System;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using AutoFixture.Xunit2;
using WorkflowEngine.Misc;
using WorkflowEngine.Helpers.Audit;
using WorkflowEngine.Core;
using WorkflowEngine.Core.Dependencies;
using WorkflowEngine.Core.Dependencies.Counters;
using WorkflowEngine.Actions.Implementations;

namespace WorkflowEngine.Test.Action.Implementations
{
    public class CounterGetActionTest
    {
        #region Constructor
        [Theory, AutoData]
        public void Constructor_CreateInstance_SuccessfullySettingItem(XElement xElement)
        {
            // Arrange
            // Act
            CounterGetAction action = new CounterGetAction(xElement);

            // Assert
            Assert.NotEqual(action, default(CounterGetAction));
            Assert.Equal(xElement, action.Item);
        }
        #endregion

        #region Execute
        [Theory, AutoData]
        public async Task Execute_CounterServiceNotFound_ExceptionExpected(
                                                Mock<IInstance> mockInstance,
                                                Mock<IContextData> mockContextData,
                                                IList<WorkflowAuditItem> list,
                                                Guid testGuid,
                                                string nameVal)
        {
            // Arrange
            mockContextData.Setup(r => r.GetCurrentRequestId())
                           .Returns(testGuid);
            mockInstance.SetupGet(v => v.AuditItems)
                        .Returns(list);
            mockInstance.Setup(svc => svc.ContextData)
                         .Returns(mockContextData.Object);

            XElement item = new XElement(
                "Test",
                new XAttribute("id", testGuid),
                new XAttribute("name", nameVal));
            CounterGetAction counterGetAction = new CounterGetAction(item)
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            WorkflowException exception = await Assert.ThrowsAsync<WorkflowException>(() => counterGetAction.ExecuteAsync());

            // Assert
            Assert.Equal($"Instance of CounterService was not registered in dependencies. Activity name: {nameVal}", exception.Message);
        }

        [Theory, AutoData]
        public async Task Execute_KeyTemplateNotFound_ExceptionExpected(
                                                Mock<IInstance> mockInstance,
                                                Mock<IContextData> mockContextData,
                                                Mock<IDependencyContainer> mockDependencyContainer,
                                                Mock<IServiceProvider> mockServiceProvider,
                                                Mock<ICounterService> mockCounterService,
                                                IList<WorkflowAuditItem> list,
                                                Guid testGuid,
                                                string nameVal,
                                                string keyTemplate,
                                                string savingPath)
        {
            // Arrange
            mockContextData.Setup(r => r.GetCurrentRequestId())
                           .Returns(testGuid);
            mockInstance.SetupGet(v => v.AuditItems)
                        .Returns(list);

            mockInstance.Setup(svc => svc.ContextData)
                         .Returns(mockContextData.Object);

            mockServiceProvider.Setup(x => x.GetService(typeof(ICounterService))).Returns(mockCounterService.Object);

            XElement item = new XElement(
                "Test",
                new XAttribute("id", testGuid),
                new XAttribute("output", savingPath),
                new XAttribute("name", nameVal));
            CounterGetAction counterGetAction = new CounterGetAction(item)
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            WorkflowException exception = await Assert.ThrowsAsync<WorkflowException>(() => counterGetAction.ExecuteAsync());

            // Assert
            Assert.Equal($"Instance of CounterService was not registered in dependencies. Activity name: {nameVal}", exception.Message);
        }

        [Theory, AutoData]
        public async Task Execute_ParametersEmpty_ExceptionExpected(
                                                Mock<IInstance> mockInstance,
                                                Mock<IContextData> mockContextData,
                                                Mock<IDependencyContainer> mockDependencyContainer,
                                                Mock<IServiceProvider> mockServiceProvider,
                                                Mock<ICounterService> mockCounterService,
                                                IList<WorkflowAuditItem> list,
                                                Guid testGuid,
                                                string nameVal,
                                                string keyTemplate,
                                                string savingPath)
        {
            // Arrange
            mockContextData.Setup(r => r.GetCurrentRequestId())
                           .Returns(testGuid);
            mockInstance.SetupGet(v => v.AuditItems)
                        .Returns(list);

            mockInstance.Setup(svc => svc.ContextData)
                         .Returns(mockContextData.Object);

            mockServiceProvider.Setup(x => x.GetService(typeof(ICounterService))).Returns(mockCounterService.Object);

            XElement item = new XElement(
                "Test",
                new XAttribute("id", testGuid),
                new XAttribute("key", keyTemplate),
                new XAttribute("output", savingPath),
                new XAttribute("name", nameVal));
            CounterGetAction counterGetAction = new CounterGetAction(item)
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            WorkflowException exception = await Assert.ThrowsAsync<WorkflowException>(() => counterGetAction.ExecuteAsync());

            // Assert
            Assert.Equal($"Instance of CounterService was not registered in dependencies. Activity name: {nameVal}", exception.Message);
        }

        [Theory, AutoData]
        public async Task Execute_GetCounterService_ExceptionExpected(
                   string name,
                   Mock<IInstance> mockInstance,
                   Mock<ICounterService> mockCounterService,
                   Mock<IContextData> mockContextData)
        {
            // Arrange
            mockInstance.Setup(svc => svc.GetDependency<ICounterService>())
                        .Returns(mockCounterService.Object);
            mockInstance.Setup(svc => svc.ContextData)
                        .Returns(mockContextData.Object);

            XElement item = new XElement("Test", new XAttribute("name", name));
            CounterGetAction customAction = new CounterGetAction(item)
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            WorkflowException exception = await Assert.ThrowsAsync<WorkflowException>(() => customAction.ExecuteAsync());

            // Assert
            Assert.Equal($"Attribute 'key' was not defined for GetCounter. Activity name: {name}", exception.Message);
        }

        [Theory, AutoData]
        public async Task Execute_GetCounterData_Successfully(
                  string key,
                  string name,
                  Mock<IInstance> mockInstance,
                  Mock<ICounterService> mockCounterService,
                  Mock<IContextData> mockContextData)
        {
            // Arrange
            List<CounterData> counterData = GetCounterData();
            mockCounterService.Setup(svc => svc.GetCounterAsync(It.IsAny<string>()))
                        .Returns(Task.FromResult(counterData));
            mockInstance.Setup(svc => svc.GetDependency<ICounterService>())
                        .Returns(mockCounterService.Object);
            mockInstance.Setup(svc => svc.ContextData)
                        .Returns(mockContextData.Object);
            mockInstance.SetupGet(v => v.AuditItems)
                        .Returns(new List<WorkflowAuditItem>());
            mockInstance.Setup(p => p.CountersCache)
                        .Returns(new Dictionary<string, List<CounterData>>());

            CounterGetAction customAction = new CounterGetAction(GetXElementStub())
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            await customAction.ExecuteAsync();

            // Assert
            mockCounterService.Verify(m => m.GetCounterAsync(It.Is<string>(v => v == "keyTest")));
            Assert.Equal(mockInstance.Object.AuditItems.Count, 1);
            Assert.True(mockInstance.Object.AuditItems[0].Info.Contains("Result: 100", StringComparison.InvariantCulture));
        }
        #endregion

        #region Property
        [Theory, AutoData]
        public void CounterGetAction_SetValue_Successfully(XElement xElement, string value)
        {
            // Arrange
            CounterGetAction action = new CounterGetAction(xElement)
            {
                Value = value
            };

            // Assert
            Assert.Equal(action.Value, value);
        }
        #endregion

        #region Stub
#pragma warning disable CA1002 // Do not expose generic lists
        public List<CounterData> GetCounterData()
#pragma warning restore CA1002 // Do not expose generic lists
        {
            return new List<CounterData>()
                   {
                      new CounterData()
                      {
                         ExpiredAt = DateTime.UtcNow,
                         TimeStamp = DateTime.UtcNow,
                         Tags = new Dictionary<string, object>()
                         {
                            { "amount", "100" },
                            { "Status", "3" }
                         }
                      }
                   };
        }

        private XElement GetXElementStub()
        {
            string xml = "<GetCounter name=\"PhoneCounter\" key=\"keyTest\" period=\"1440\" filter = \"'Status' = '3'\" function=\"sum\" tag=\"amount\" output=\"PhoneCounter_Result\" ignoreCache=\"false\">\r\n" +
                         "    <parameter type=\"appData\" name=\"value\" value=\"requisites.phone.value\" />\r\n" +
                         "</GetCounter>";

            return XElement.Parse(xml);
        }

        private XElement GetBrokenXElementStub()
        {
            string xml = "<SetCounter name=\"Store\" key=\"keyTest\" ttl=\"123\" >\r\n" +
                "    <parameter type=\"appData\" name=\"value\" value=\"requisites.phone.value\" />\r\n" +
                "  </SetCounter>";

            return XElement.Parse(xml);
        }
        #endregion
    }
}
