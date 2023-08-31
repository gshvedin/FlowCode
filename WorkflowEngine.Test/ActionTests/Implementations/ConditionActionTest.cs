using System.Xml.Linq;
using Xunit;
using WorkflowEngine.Actions.Implementations;

namespace WorkflowEngine.Test.Action.Implementations
{
    public class ConditionActionTest
    {
        #region Constructor
        [Fact]
        public void Constructor_ConditionAction_CheckItem()
        {
            // Arrange
            XElement item = new XElement("TestName", "TestValue");

            // Act
            ConditionAction res = new ConditionAction(item);

            // Assert
            Assert.Equal(res.Item, item);
        }
        #endregion
    }
}
