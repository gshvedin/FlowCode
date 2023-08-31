using AutoFixture.Xunit2;
using WorkflowEngine.Core.WorkflowProcedures;
using Xunit;

namespace WorkflowEngine.Test.CoreTests
{
    public class WorkflowProcedureItemTest
    {
        [Theory, AutoData]
        public void Name_SetValue_Successfully(string name)
        {
            // Arrange
            WorkflowProcedureItem item = new WorkflowProcedureItem
            {
                // Act
                Name = name,
            };

            // Assert
            Assert.Equal(name, item.Name);
        }

        [Theory, AutoData]
        public void Definition_SetValue_Successfully(string definition)
        {
            // Arrange
            WorkflowProcedureItem item = new WorkflowProcedureItem
            {
                // Act
                Definition = definition,
            };

            // Assert
            Assert.Equal(definition, item.Definition);
        }
    }
}
