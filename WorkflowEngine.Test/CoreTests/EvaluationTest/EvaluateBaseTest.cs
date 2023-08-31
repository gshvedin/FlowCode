using System.Xml.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Xunit.Abstractions;
using AutoFixture.Xunit2;
using WorkflowEngine.Misc;
using WorkflowEngine.Core;
using WorkflowEngine.Core.Evaluation;

namespace WorkflowEngine.Test.CoreTests.EvaluationTest
{
    public class EvaluateBaseTest
    {
        private readonly ITestOutputHelper output;

        public EvaluateBaseTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory, AutoData]
        public void Constructor_CreateInstanceAndSetItemAndData_SuccessfullySetting(
                                                             XElement item,
                                                             Mock<IInstance> data)
        {
            // Arrange
            // Act
            EvaluateBase evaluateBase = new EvaluateBase(item, data.Object);

            // Assert
            Assert.Equal(item, evaluateBase.Item);
            Assert.Equal(data.Object, evaluateBase.Instance);
        }

        [Theory, AutoData]
        public async Task Evaluate_Python_Successfully(Mock<IInstance> data)
        {
            // Arrange
            XElement item = new XElement("Test", new XAttribute("expression", "2*2"), new XAttribute("lang", "Python"));
            EvaluateBase evaluateBase = new EvaluateBase(item, data.Object);

            // Act
            string res = await evaluateBase.EvaluateAsync();

            // Assert
            Assert.Equal("4", res);
        }

        [Theory, AutoData]
        public async Task Evaluate_XPath_Successfully(Mock<IInstance> data)
        {
            // Arrange
            XElement item = new XElement("Test", new XAttribute("expression", "2*2"), new XAttribute("lang", "XPath"));
            EvaluateBase evaluateBase = new EvaluateBase(item, data.Object);

            // Act
            string res = await evaluateBase.EvaluateAsync();

            // Assert
            Assert.Equal("4", res);
        }

        [Theory, AutoData]
        public async Task Evaluate_XPathExpression_Successfully(Mock<IInstance> mockInstance)
        {
            // Arrange
            string s = "<Condition>" +
                          "<test expression = \"{0} > 1\" >" +
                             "<parameter type = \"appData\" value = \"testName\"/> " +
                          "</test>" +
                          "<iftrue>" +
                             "<Result name = \"BR_01\" expression = \"1\"/>" +
                          "</iftrue>" +
                          "<iffalse>" +
                             "<Result name = \"BR_01\" expression = \"0\"/>" +
                          "</iffalse>" +
                        "</Condition>";

            IContextData contextData = new ContextData("{ \"testName\": 2 }", mockInstance.Object);
            mockInstance.SetupGet(v => v.ContextData)
                        .Returns(contextData);
            XElement item = XElement.Parse(s);
            EvaluateBase evaluateBase = new EvaluateBase(item, mockInstance.Object);

            // Act
            string res = await evaluateBase.EvaluateAsync();

            // Assert
            Assert.Equal("True", res);
        }

        [Theory]
        [InlineData(0, 0, "False")]
        [InlineData(0, 1, "False")]
        [InlineData(0, 2, "False")]
        [InlineData(0, 3, "False")]
        [InlineData(0, 4, "False")]
        [InlineData(0, 5, "True")]
        [InlineData(0, 10, "True")]
        [InlineData(1, 0, "False")]
        [InlineData(1, 1, "False")]
        [InlineData(1, 2, "False")]
        [InlineData(1, 3, "False")]
        [InlineData(1, 4, "False")]
        [InlineData(1, 5, "False")]
        [InlineData(2, 0, "False")]
        [InlineData(2, 4, "False")]
        [InlineData(2, 5, "False")]
        public async Task Evaluate_XPathExpressionMultyParameters_Successfully(int success_t, int fails_t, string expectedRes)
        {
            // Arrange
            string s = "<Condition>" +
                          "<test expression = \"{0} &lt; 1 and {1} > 4\" >" + // &lt;
                             "<parameter type = \"appData\" value = \"success_t\"/> " +
                             "<parameter type = \"appData\" value = \"fails_t\"/> " +
                          "</test>" +
                          "<iftrue>" +
                             "<Result name = \"BR_01\" expression = \"1\"/>" +
                          "</iftrue>" +
                          "<iffalse>" +
                             "<Result name = \"BR_01\" expression = \"0\"/>" +
                          "</iffalse>" +
                        "</Condition>";

            Mock<IInstance> mockInstance = new Mock<IInstance>();
            IContextData contextData = new ContextData("{ \"success_t\": " + success_t + ", \"fails_t\": " + fails_t + " }", mockInstance.Object);
            mockInstance.SetupGet(v => v.ContextData)
                        .Returns(contextData);
            XElement item = XElement.Parse(s);
            EvaluateBase evaluateBase = new EvaluateBase(item, mockInstance.Object);

            // Act
            string res = await evaluateBase.EvaluateAsync();

            if (res != expectedRes)
            {
                output.WriteLine($"{success_t} / {fails_t} ");
            }

            // Assert
            Assert.Equal(expectedRes, res);
        }

        #region EvaluateBool
        [Theory, AutoData]
        public async Task EvaluateBool_XPath_Successfully(Mock<IInstance> data)
        {
            // Arrange
            XElement item = new XElement("Test", new XAttribute("expression", "1=1"), new XAttribute("lang", "XPath"));
            EvaluateBase evaluateBase = new EvaluateBase(item, data.Object);

            // Act
            bool res = await evaluateBase.EvaluateBoolAsync();

            // Assert
            Assert.True(res);
        }

        [Theory, AutoData]
        public async Task EvaluateBool_ReturnNotBoolValue_ThhrowWorkflowException(Mock<IInstance> data)
        {
            // Arrange
            XElement item = new XElement("Test", new XAttribute("expression", "123"), new XAttribute("lang", "XPath"));
            EvaluateBase evaluateBase = new EvaluateBase(item, data.Object);

            // Act
            WorkflowException exception = await Assert.ThrowsAsync<WorkflowException>(() => evaluateBase.EvaluateBoolAsync());

            // Assert
            Assert.Equal("Unable to cast '123' to boolean.", exception.Message);
        }
        #endregion

        [Theory, AutoData]
        public void ToString_ReturnNotBoolValue_ThrowWorkflowException(
                                                        Mock<IInstance> data,
                                                        XElement item)
        {
            // Arrange
            EvaluateBase evaluateBase = new EvaluateBase(item, data.Object);

            // Act
            string res = evaluateBase.ToString();

            // Assert
            Assert.Equal("{\"Expression\":null,\"Result\":null,\"ExecutionTime\":0}", res);
        }
    }
}
