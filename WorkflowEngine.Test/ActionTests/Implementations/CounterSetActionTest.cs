using AutoFixture.Xunit2;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using WorkflowEngine.Actions.Implementations;
using WorkflowEngine.Core;
using WorkflowEngine.Core.Dependencies.Counters;
using WorkflowEngine.Helpers.Audit;
using WorkflowEngine.Misc;
using Xunit;

namespace WorkflowEngine.Test.Action.Implementations
{
    public class CounterSetActionTest
    {
        #region Constructor
        [Theory, AutoData]
        public void Constructor_CreateInstance_SuccessfullySettingItem(XElement xElement)
        {
            // Arrange
            // Act
            CounterSetAction action = new CounterSetAction(xElement);

            // Assert
            Assert.NotEqual(action, default(CounterSetAction));
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
            CounterSetAction counterGetAction = new CounterSetAction(item)
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
            CounterSetAction customAction = new CounterSetAction(item)
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            WorkflowException exception = await Assert.ThrowsAsync<WorkflowException>(() => customAction.ExecuteAsync());

            // Assert
            Assert.Equal($"Attribute 'key' was not defined for SetCounter. Activity name: {name}", exception.Message);
        }

        [Theory, AutoData]
        public async Task Execute_SetCounterData_Successfully(
                  string key,
                  string name,
                  Mock<IInstance> mockInstance,
                  Mock<ICounterService> mockCounterService,
                  Mock<IContextData> mockContextData)
        {
            // Arrange
            mockCounterService.Setup(svc => svc.SetCounterAsync(It.IsAny<string>(), It.IsAny<CounterData>(), It.IsAny<int>()))
                        .Returns(Task.FromResult(true));
            mockInstance.Setup(svc => svc.GetDependency<ICounterService>())
                        .Returns(mockCounterService.Object);
            mockInstance.Setup(svc => svc.ContextData)
                        .Returns(mockContextData.Object);
            mockInstance.SetupGet(v => v.AuditItems)
                        .Returns(new List<WorkflowAuditItem>());

            CounterSetAction customAction = new CounterSetAction(GetXElementStub())
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            await customAction.ExecuteAsync();

            // Assert
            mockCounterService.Verify(m => m.SetCounterAsync(
                It.Is<string>(v => v == "keyTest"),
                It.Is<CounterData>(v => v.Tags["transaction_type"].ToString() == "Deposit"
                                     && v.Tags["transaction_id"] == null
                                     && v.Tags["order_id"] == null),
                It.Is<int>(v => v == 43200)));

            Assert.Equal(mockInstance.Object.AuditItems.Count, 1);
            Assert.True(mockInstance.Object.AuditItems[0].Info.Contains("Result: True", StringComparison.InvariantCulture));
        }

        [Theory, AutoData]
        public async Task Execute_ErrorParseXml_ExceptionExpected(
                 string key,
                 string name,
                 Mock<IInstance> mockInstance,
                 Mock<ICounterService> mockCounterService,
                 Mock<IContextData> mockContextData)
        {
            // Arrange
            mockCounterService.Setup(svc => svc.SetCounterAsync(It.IsAny<string>(), It.IsAny<CounterData>(), It.IsAny<int>()))
                        .Returns(Task.FromResult(true));
            mockInstance.Setup(svc => svc.GetDependency<ICounterService>())
                        .Returns(mockCounterService.Object);
            mockInstance.Setup(svc => svc.ContextData)
                        .Returns(mockContextData.Object);
            mockInstance.SetupGet(v => v.AuditItems)
                        .Returns(new List<WorkflowAuditItem>());

            CounterSetAction customAction = new CounterSetAction(GetBrokenXElementStub())
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            await customAction.ExecuteAsync();

            // Assert
            Assert.Equal(mockInstance.Object.AuditItems.Count, 2);
            Assert.True(mockInstance.Object.AuditItems[0].Info.StartsWith("Exception thrown while Store executing: System.NullReferenceException: Object reference not set to an instance of an object.", StringComparison.InvariantCulture));
            Assert.True(mockInstance.Object.AuditItems[1].Info.Contains("Result: False", StringComparison.InvariantCulture));
        }

        #endregion

        #region Property
        [Theory, AutoData]
        public void CounterSetAction_Constructor_Successfully(XElement xElement, string value)
        {
            // Arrange
            CounterSetAction action = new CounterSetAction(xElement)
            {
                Value = value
            };

            // Assert
            Assert.Equal(action.Value, value);
        }
        #endregion

        #region Stub
        private XElement GetXElementStub()
        {
            string xml = "<SetCounter name=\"Store\" key=\"keyTest\" ttl=\"43200\" >\r\n" +
                "    <parameter type=\"appData\" name=\"value\" value=\"requisites.phone.value\" />\r\n" +
                "    <Tags>\r\n" +
                "      <parameter type=\"constant\" name=\"transaction_type\" value=\"Deposit\" />\r\n" +
                "      <parameter type=\"appData\" name=\"transaction_id\" value=\"transaction_id\" />\r\n" +
                "      <parameter type=\"appData\" name=\"order_id\" value=\"order_id\" />\r\n" +
                "    </Tags>\r\n" +
                "  </SetCounter>";

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