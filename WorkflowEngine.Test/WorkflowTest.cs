using AutoFixture.Xunit2;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using WorkflowEngine.Core;
using WorkflowEngine.Core.Dependencies;
using WorkflowEngine.Helpers.Audit;
using Xunit;

namespace WorkflowEngine.Test
{
    public class WorkflowTest
    {
        #region Execute

        [Theory, AutoData]
        internal void Execute_SetNullWorkflowDefinition_ThrowXmlException(Mock<IDependencyContainer> mockServiceProvider)
        {
            // Act
            XmlException exception = Assert.Throws<XmlException>(() => new Workflow(string.Empty, mockServiceProvider.Object));

            // Assert
            Assert.Equal("Root element is missing.", exception.Message);
        }

        [Theory, AutoData]
        internal async Task Execute_SetData_Successful(
                            Guid requestId,
                            Mock<IDependencyContainer> mockDependencyContainer,
                            Mock<IInstance> mockInstance,
                            Mock<IContextData> mockContextData,
                            Mock<IWorkflowContext> mockWorkflowContext,
                            Mock<IWorkflowAuditService> mockAuditService,
                            IList<WorkflowAuditItem> list)
        {
            // Arrange
            mockAuditService.Setup(m => m.AddAuditItems(It.IsAny<IList<WorkflowAuditItem>>(), It.IsAny<Dictionary<string, object>>()))
                            .Returns(Task.CompletedTask);
            mockInstance.SetupGet(v => v.AuditItems)
                        .Returns(list);
            mockInstance.Setup(m => m.ContextData)
                        .Returns(mockContextData.Object);
            mockInstance.Setup(svc => svc.GetDependency<IWorkflowAuditService>())
                       .Returns(mockAuditService.Object);

            // Arrange
            Dictionary<string, object> dictionaryTest = new Dictionary<string, object>
            {
                { "RequestId", requestId }
            };

            mockInstance.SetupGet(v => v.DC)
                        .Returns(mockDependencyContainer.Object);
            mockDependencyContainer.SetupGet(v => v.MetaInfo)
                                    .Returns(dictionaryTest);

            ContextData contextData = new ContextData("{}", mockInstance.Object);

            string workflowDefinition = "<Workflow><Start/><Finish/></Workflow>";
            mockInstance.Setup(x => x.WorkflowDefinition)
                .Returns(workflowDefinition);
            mockInstance.Setup(x => x.ContextData)
                .Returns(contextData);

            Workflow workflow = new Workflow(workflowDefinition, mockInstance.Object)
            {
                WorkflowContext = mockWorkflowContext.Object,
            };

            // Act
            await workflow.ExecuteAsync("{\"testKey\":\"testVal\"}");

            // Assert
            mockInstance.Verify(m => m.ContextData);
            mockWorkflowContext.Verify(m => m.ExecuteAsync());
        }
        #endregion

        #region SetDependencies
        [Theory, AutoData]
        internal void SetDependencies_Set_Successful(
                                    Mock<IInstance> mockInstance,
                                    Mock<IDependencyContainer> mockServiceProvider)
        {
            // Arrange
            mockInstance.Setup(m => m.SetDependencies(It.IsAny<IDependencyContainer>()))
                        .Verifiable();
            Workflow workflow = new Workflow(null, mockInstance.Object);

            // Act
            workflow.SetDependencies(mockServiceProvider.Object);

            // Assert
            mockInstance.Verify(m => m.SetDependencies(It.Is<IDependencyContainer>(v => v == mockServiceProvider.Object)));
        }
        #endregion
    }
}
