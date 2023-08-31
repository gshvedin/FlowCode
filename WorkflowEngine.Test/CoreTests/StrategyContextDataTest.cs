using System;
using System.Text;
using Xunit;
using WorkflowEngine.Core.Strategies;

namespace WorkflowEngine.Test.CoreTests
{
    public class StrategyContextDataTest
    {
        [Fact]
        public void ToXml_ParseStrategyContextDataToXml_Successfully()
        {
            // Arrange
            StrategyContextData strategyContextData = GetStrategyContextDataStub();

            // Act
            string res = strategyContextData.ToXml();

            // Assert
            Assert.Equal(
                GetXmlStub().Replace("\r", string.Empty, StringComparison.CurrentCulture),
                res.Replace("\r", string.Empty, StringComparison.CurrentCulture));
        }

        [Fact]
        public void ToJson_ParseStrategyContextDataToJson_Successfully()
        {
            // Arrange
            StrategyContextData strategyContextData = GetStrategyContextDataStub();

            // Act
            string res = strategyContextData.ToJson();

            // Assert
            Assert.Equal(GetJsonStub(), res);
        }

        [Fact]
        public void EvaluateXPath_TestXmlValue()
        {
            // Arrange
            StrategyContextData strategyContextData = GetStrategyContextDataStub();

            // Act
            string res = strategyContextData.EvaluateXPath("StrategyResult/testKey1/text()");

            // Assert
            Assert.Equal("testValue1", res);
        }

        [Fact]
        public void EvaluateXPath_TestAggreagateValue()
        {
            // Arrange
            StrategyContextData strategyContextData = GetStrategyContextDataStub();

            // Act
            string res = strategyContextData.EvaluateXPath("count(StrategyResult/*)");

            // Assert
            Assert.Equal("2", res);
        }

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

        private static string GetXmlStub()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<StrategyResult>\r\n  ");
            stringBuilder.Append("<testKey1>testValue1</testKey1>\r\n  ");
            stringBuilder.Append("<testKey2>testValue2</testKey2>\r\n");
            stringBuilder.Append("</StrategyResult>");

            return stringBuilder.ToString();
        }

        private static string GetJsonStub()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("{");
            stringBuilder.Append("\"testKey1\": \"testValue1\",");
            stringBuilder.Append("\"testKey2\": \"testValue2\"");
            stringBuilder.Append("}");

            return stringBuilder.ToString();
        }
        #endregion
    }
}
