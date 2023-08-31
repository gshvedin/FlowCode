using System;
using System.Xml.Linq;
using Moq;
using Xunit;
using AutoFixture.Xunit2;
using WorkflowEngine.Core;
using WorkflowEngine.Helpers.Audit;
using WorkflowEngine.Actions.Implementations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkflowEngine.Test.Action.Implementations
{
    public class DelayActionTest
    {
        #region Constructor
        [Theory, AutoData]
        public void Constructor_CreateInstance_SuccessfullySettingItem(XElement xElement)
        {
            // Arrange

            // Act
            DelayAction delayAction = new DelayAction(xElement);

            // Assert
            Assert.NotEqual(delayAction, default(DelayAction));
            Assert.Equal(xElement, delayAction.Item);
        }
        #endregion

        #region Execute
        [Theory, AutoData]
        public async Task Execute_AddAudit_SuccessfulCheckSettingData(
                                                Mock<IInstance> mockInstance,
                                                Mock<IContextData> mockContextData,
                                                IList<WorkflowAuditItem> list,
                                                Guid testGuid,
                                                string nameVal,
                                                int msVal)
        {
            // Arrange
            mockContextData.Setup(r => r.GetCurrentRequestId())
                           .Returns(testGuid);
            mockInstance.SetupGet(v => v.AuditItems)
                        .Returns(list);
            mockInstance.Setup(svc => svc.ContextData)
                         .Returns(mockContextData.Object);

            XElement item = new XElement("Test", new XAttribute("id", testGuid), new XAttribute("name", nameVal), new XAttribute("ms", msVal));
            DelayAction delayAction = new DelayAction(item)
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            await delayAction.ExecuteAsync();

            // Assert
            WorkflowAuditItem workflow = list.SingleOrDefault(v => v.NodeName == nameVal);
            Assert.Equal($"Was delayed for {msVal} ms", workflow.Info);

            mockContextData.Verify(r => r.GetCurrentRequestId());
        }

        [Theory, AutoData]
        public async Task Execute_SetDelayStr_SetDefaultValue(
                                        Mock<IInstance> mockInstance,
                                        Mock<IContextData> mockContextData,
                                        IList<WorkflowAuditItem> list,
                                        Guid testGuid)
        {
            // Arrange
            mockInstance.SetupGet(v => v.AuditItems)
                        .Returns(list);
            mockContextData.Setup(r => r.GetCurrentRequestId())
                           .Returns(testGuid);
            mockInstance.Setup(svc => svc.ContextData)
                        .Returns(mockContextData.Object);

            XElement item = new XElement("Test", new XAttribute("ms", "test"));
            DelayAction delayAction = new DelayAction(item)
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
            await delayAction.ExecuteAsync();

            // Assert
            WorkflowAuditItem workflow = list.SingleOrDefault(v => v.NodeName == delayAction.GetType().Name);
            Assert.Equal("Was delayed for 500 ms", workflow.Info);

            mockContextData.Verify(r => r.GetCurrentRequestId());
        }
        #endregion
    }
}
