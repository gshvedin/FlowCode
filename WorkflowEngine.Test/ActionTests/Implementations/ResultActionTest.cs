using System;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Moq;
using Xunit;
using AutoFixture.Xunit2;
using WorkflowEngine.Core;
using WorkflowEngine.Helpers.Audit;
using WorkflowEngine.Actions.Implementations;

namespace WorkflowEngine.Test.Action.Implementations
{
    public class ResultActionTest
    {
        #region Constructor
        [Theory, AutoData]
        public void Constructor_CreateInstance_SuccessfullySettingItem(XElement xElement)
        {
            // Arrange
            // Act
            ResultAction resultAction = new ResultAction(xElement);

            // Assert
            Assert.NotEqual(resultAction, default(ResultAction));
            Assert.Equal(xElement, resultAction.Item);
        }
        #endregion

        #region Execute
        [Theory, AutoData]
        internal async Task Execute_AddAudit_SuccessfulCheckSettingData(
                                                    Mock<IInstance> mockInstance,
                                                    Mock<IContextData> mockContextData,
                                                    IList<WorkflowAuditItem> list,
                                                    string nameVal,
                                                    Guid testGuid)
        {
            // Arrange
            mockInstance.SetupGet(v => v.AuditItems)
                        .Returns(list);
            mockContextData.Setup(r => r.GetCurrentRequestId())
                            .Returns(testGuid);
            mockInstance.Setup(svc => svc.ContextData)
                        .Returns(mockContextData.Object);

            XElement item = new XElement("Test", new XAttribute("expression", nameVal));
            ResultAction delayAction = new ResultAction(item)
            {
                CurrentInstance = mockInstance.Object
            };

            // Act
           await delayAction.ExecuteAsync();

            // Assert
            Assert.NotNull(delayAction.Value);

            WorkflowAuditItem workflow = list.SingleOrDefault(v => v.NodeName == delayAction.GetType().Name);
            Assert.NotNull(workflow.Info);

            mockContextData.Verify(r => r.GetCurrentRequestId());
        }
        #endregion
    }
}
