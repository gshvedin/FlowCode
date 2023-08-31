using AutoFixture.Xunit2;
using System.Collections.Generic;
using WorkflowEngine.Core.Dependencies.Lists;
using Xunit;

namespace WorkflowEngine.Test
{
    public class ListItemTest
    {
        [Theory, AutoData]
        public void Id_SetValue_Successfully(int id)
        {
            // Arrange
            ListItem listItem = new ListItem
            {
                // Act
                Id = id,
            };

            // Assert
            Assert.Equal(id, listItem.Id);
        }

        [Theory, AutoData]
        public void Name_SetValue_Successfully(string name)
        {
            // Arrange
            ListItem listItem = new ListItem
            {
                // Act
                Name = name,
            };

            // Assert
            Assert.Equal(name, listItem.Name);
        }

        [Theory, AutoData]
        public void ExtendedData_SetValue_Successfully(Dictionary<string, string> extendedData)
        {
            // Arrange
            ListItem listItem = new ListItem
            {
                // Act
                ExtendedData = extendedData,
            };

            // Assert
            Assert.Equal(extendedData, listItem.ExtendedData);
        }
    }
}
