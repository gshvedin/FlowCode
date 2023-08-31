using System.Xml;
using Xunit;
using WorkflowEngine.Misc;
using WorkflowEngine.Core.Evaluation;
using WorkflowEngine.Core.Strategies;

namespace WorkflowEngine.Test.CoreTests.EvaluationTest
{
    public class EvaluatorTest
    {
        #region EvaluatePython
        [Fact]
        internal void EvaluatePython_OnePlusTwo_ReturnThree()
        {
            // Arrange
            // Act
            string res = Evaluator.EvaluatePython("1+2");

            // Assert
            Assert.Equal("3", res);
        }

        [Fact]
        internal void EvaluatePython_SetNotValidData_ThrowWorkflowException()
        {
            // Arrange
            // Act
            WorkflowException exception = Assert.Throws<WorkflowException>(() => Evaluator.EvaluatePython("ds-sd=dsds8*dsd"));

            // Assert
            Assert.Equal("Error of Python expression:\ncan't assign to operator\nExpression text:ds-sd=dsds8*dsd", exception.Message);
        }
        #endregion

        #region EvaluateXPath
        [Fact]
        internal void EvaluateXPath_OnePlusTwo_ReturnThree()
        {
            // Arrange
            // Act
            string res = Evaluator.EvaluateXPath("1+2");

            // Assert
            Assert.Equal("3", res);
        }

        [Fact]
        internal void EvaluateXPath_SetNotValidData_ThrowWorkflowException()
        {
            // Arrange
            // Act
            WorkflowException exception = Assert.Throws<WorkflowException>(() => Evaluator.EvaluateXPath("throw new Exception(\"Test\");"));

            // Assert
            string s = exception.Message;
            Assert.Equal(
                "Error of XPath expression:\n'throw new Exception(\"Test\");' has an invalid token.\nExpression text:throw new Exception(\"Test\");",
                exception.Message);
        }
        #endregion

        #region EvaluateXPath2
        [Fact]
        internal void EvaluateXPath2_()
        {
            // Arrange
            // Act
            string res = Evaluator.EvaluateXPath(GetXmlStub(), "StrategyResult/testKey1/text()");

            // Assert
            Assert.Equal("testValue1", res);
        }

        [Fact]
        internal void EvaluateXPath2_Successfully()
        {
            // Arrange, Act
            string res = Evaluator.EvaluateXPath("fn:replace('abracadabra', 'bra', '*')", true);

            // Assert
            Assert.Equal("a*cada*", res);
        }

        [Fact]
        internal void EvaluateXPath2_RegexContains_Successfully()
        {
            // Arrange, Act
            string res = Evaluator.EvaluateXPath("contains('empty parameter', '')", true);

            // Assert
            Assert.Equal("True", res);
        }

        [Fact]
        internal void EvaluateXPath2_OnePlusTwo_ReturnThree()
        {
            // Arrange
            // Act
            string res = Evaluator.EvaluateXPath("1+2");

            // Assert
            Assert.Equal("3", res);
        }

        [Fact]
        internal void EvaluateXPath2_SetNotValidData_ThrowWorkflowException()
        {
            // Arrange
            // Act
            WorkflowException exception = Assert.Throws<WorkflowException>(() => Evaluator.EvaluateXPath(GetXmlStub(), "throw new Exception(\"Test\");"));

            // Assert
            Assert.Equal(
                "Error of XPath expression:\n'.//throw new Exception(\"Test\");' has an invalid token.\nExpression text:.//throw new Exception(\"Test\");",
                exception.Message);
        }

        #endregion

        #region Stubs
        private static StrategyContextData GetStrategyContextDataStub()
        {
            StrategyContextData strategyContextData = new StrategyContextData
            {
                { "testKey1", "testValue1" },
                { "testKey2", "testValue2" }
            };

            return strategyContextData;
        }

        private static XmlDocument GetXmlStub()
        {
            StrategyContextData strategyContext = GetStrategyContextDataStub();
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(strategyContext.ToXml());

            return xDoc;
        }

        #endregion
    }
}
