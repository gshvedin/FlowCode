using AutoFixture;
using AutoFixture.Xunit2;
using Moq;
using WorkflowEngine.Core.Dependencies;
using WorkflowEngine.Core.WorkflowProcedures;
using Xunit;
using Xunit.Abstractions;

namespace WorkflowEngine.Test.CoreTests
{
    public class WorkflowProceduresListTest
    {
        private readonly ITestOutputHelper output;
        private readonly Fixture fixture;

        public WorkflowProceduresListTest(ITestOutputHelper output)
        {
            this.output = output;
            fixture = new Fixture();
        }

        [Theory, AutoData]
        public void Constructor_AllDataCorrect_ExpectedElementsCount()
        {
            // Arrange
            string definition = GetDefinition(true);
            Instance instance = new Instance()
            {
                WorkflowDefinition = definition
            };

            // Act
            WorkflowProceduresList list = new WorkflowProceduresList(definition, instance);

            // Assert
            Assert.Equal(1, list.Count);
        }

        [Theory, AutoData]
        public void Constructor_DefinitionIsNull_ExpectedEmptyElements()
        {
            // Arrange
            string definition = null;
            Instance instance = new Instance()
            {
                WorkflowDefinition = definition
            };

            // Act
            WorkflowProceduresList list = new WorkflowProceduresList(definition, instance);

            // Assert
            Assert.Empty(list);
        }

        [Theory, AutoData]
        public void GetWorkflowProcedure_AllDataCorrect_ExpectedWorkflowProcedureReturned(
              Mock<IDependencyContainer> mockDependencyContainer,
              Mock<IInstance> mockInstance)
        {
            // Arrange
            string procedurename = "TestProcedure";
            string definition = GetDefinition(true);
            string expectedProcedureDefinition = GetProcedure();

            mockInstance.SetupGet(v => v.DC)
                .Returns(mockDependencyContainer.Object);

            Mock<IWorkflowProcedureContainer> workflowContainer = new Mock<IWorkflowProcedureContainer>();
            workflowContainer.Setup(x => x.GetWorkflowProcedure(It.Is<string>(v => v == procedurename))).Returns(definition);
            mockDependencyContainer.Setup(x => x.ServiceProvider.GetService(typeof(IWorkflowProcedureContainer))).Returns(workflowContainer.Object);
            mockInstance.Setup(x => x.DC).Returns(mockDependencyContainer.Object);

            WorkflowProceduresList list = new WorkflowProceduresList(definition, mockInstance.Object);

            // Act
            string result = list.GetWorkflowProcedure(procedurename);

            // Assert
            Assert.Equal(expectedProcedureDefinition, result);
        }

        [Theory, AutoData]
        public void GetWorkflowProcedure_NoneProcedureProvided_ExpectedNullWorkflowProcedureReturned(
              Mock<IDependencyContainer> mockDependencyContainer,
              Mock<IInstance> mockInstance)
        {
            // Arrange
            string procedurename = "TestProcedure";
            string definition = GetDefinition(false);
            string expectedProcedureDefinition = null;

            mockInstance.SetupGet(v => v.DC)
                .Returns(mockDependencyContainer.Object);

            Mock<IWorkflowProcedureContainer> workflowContainer = new Mock<IWorkflowProcedureContainer>();
            workflowContainer.Setup(x => x.GetWorkflowProcedure(It.Is<string>(v => v == procedurename))).Returns(definition);
            mockDependencyContainer.Setup(x => x.ServiceProvider.GetService(typeof(IWorkflowProcedureContainer))).Returns(workflowContainer.Object);
            mockInstance.Setup(x => x.DC).Returns(mockDependencyContainer.Object);

            WorkflowProceduresList list = new WorkflowProceduresList(definition, mockInstance.Object);

            // Act
            string result = list.GetWorkflowProcedure(procedurename);

            // Assert
            Assert.Equal(expectedProcedureDefinition, result);
        }

        private string GetDefinition(bool addProcedure)
        {
            return "<Workflow>" +
                "<Start/>" +
                "<WorkflowProcedures>" +
                (addProcedure ? GetProcedure() : string.Empty) +
                "</WorkflowProcedures>" +
                "<Finish/>" +
                "</Workflow>";
        }

        private string GetProcedure()
        {
            return "<WorkflowProcedure name=\"TestProcedure\" />";
        }
    }
}
