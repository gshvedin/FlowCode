using System.Xml.Linq;
using Xunit;
using AutoFixture.Xunit2;
using WorkflowEngine.Helpers;

namespace WorkflowEngine.Test.HelpersTest
{
    public class DataExtensionsTest
    {
        [Theory, AutoData]
        public void GetAttribute_GetValueByAttributeName_Successful(string atributName, string testValue)
        {
            // Arrange
            XElement item = new XElement("Test", new XAttribute(atributName, testValue));

            // Act
            string val = item.GetAttribute(atributName);

            // Assert
            Assert.Equal(testValue, val);
        }
    }
}
