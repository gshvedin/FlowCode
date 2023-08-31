using System;
using System.Xml.Linq;
using Xunit;
using AutoFixture.Xunit2;
using AutoFixture;
using WorkflowEngine.Actions;
using WorkflowEngine.Actions.Implementations;
using Moq;

namespace WorkflowEngine.Test.Action
{
    public class CoreActionsFactoryTest
    {
        [Theory, AutoData]
        public void SupportsActionType_Check_ReturnTrue(CoreActionsFactory coreActionsFactory)
        {
            // Arrange
            string typeName = "Start";

            // Act
            bool res = coreActionsFactory.SupportsActionType(typeName);

            // Assert
            Assert.True(res);
        }

        [Theory, AutoData]
        public void SupportsActionType_Check_ReturnFalse(CoreActionsFactory coreActionsFactory)
        {
            // Arrange
            string typeName = "Qwert";

            // Act
            bool res = coreActionsFactory.SupportsActionType(typeName);

            // Assert
            Assert.False(res);
        }

        [Theory]
        [InlineData("Start", typeof(StartProcess))]
        [InlineData("Finish", typeof(FinishProcess))]
        [InlineData("Condition", typeof(ConditionAction))]
        [InlineData("Connector", typeof(ConnectorAction))]
        [InlineData("Strategy", typeof(StrategyAction))]
        [InlineData("DataStore", typeof(DataStoreAction))]
        [InlineData("SelectCase", typeof(SelectCaseAction))]
        [InlineData("UserTask", typeof(UserTaskAction))]
        [InlineData("CustomAction", typeof(CustomAction))]
        [InlineData("Result", typeof(ResultAction))]
        [InlineData("GoTo", typeof(GoToAction))]
        [InlineData("Parallel", typeof(ParallelAction))]
        [InlineData("Delay", typeof(DelayAction))]
        [InlineData("DataTransform", typeof(DataTransformAction))]
        [InlineData("Point", typeof(PointAction))]
        public void CreateActions_SetItem_Successfully(string actionName, Type type)
        {
            // Arrange
            Fixture fixture = new Fixture();
            CoreActionsFactory coreActionsFactory = fixture.Create<CoreActionsFactory>();
            XElement testXElement = new XElement(actionName, $"Test value: {type.Name}");

            Mock<IInstance> mockInstance = new Mock<IInstance>();

            // Act
            WorkflowActionBase res = coreActionsFactory.CreateAction(testXElement, mockInstance.Object);

            // Assert
            Assert.IsType(type, res);
            Assert.Equal(mockInstance.Object, res.CurrentInstance);
            Assert.Equal(testXElement, res.Item);
        }

        [Theory]
        [InlineData("Qwert", typeof(DummyAction))]
        public void CreateActionDummyAction_NotSetItem_Successfully(string actionName, Type type)
        {
            // Arrange
            Fixture fixture = new Fixture();
            CoreActionsFactory coreActionsFactory = fixture.Create<CoreActionsFactory>();
            XElement testXElement = new XElement(actionName, $"Test value: {type.Name}");

            Mock<IInstance> mockInstance = new Mock<IInstance>();

            // Act
            WorkflowActionBase res = coreActionsFactory.CreateAction(testXElement, mockInstance.Object);

            // Assert
            Assert.IsType(type, res);
            Assert.Equal(mockInstance.Object, res.CurrentInstance);
            Assert.Null( res.Item);
        }
    }
}
