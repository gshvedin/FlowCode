using AutoFixture;
using AutoFixture.Xunit2;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WorkflowEngine.Core.Dependencies.Counters;
using WorkflowEngine.Misc;
using Xunit;
using Xunit.Abstractions;

namespace WorkflowEngine.Test.CoreTests.EvaluationTest
{
    public class CounterEvaluatorTests
    {
        private readonly ITestOutputHelper output;
        private readonly Fixture fixture;

        public CounterEvaluatorTests(ITestOutputHelper output)
        {
            this.output = output;
            fixture = new Fixture();
        }

        [Theory, AutoData]
        public void Evaluate_EnumIsNotCorrect_ExceptionReceived(
            IEnumerable<CounterData> counterData,
            string function,
            string tag,
            string filter,
            int period)
        {
            // Arrange
            // Act

            // Assert
            WorkflowException exception = Assert.Throws<WorkflowException>(() => new CounterEvaluator().Evaluate(counterData, null, tag, filter, period));

            IEnumerable<KeyValuePair<string, object>> set = counterData
                        .Where(t => true && t.TimeStamp > DateTime.UtcNow.AddMinutes(-1 * period))
                        .SelectMany(t => t.Tags)
                        .Where(i => i.Key == tag && i.Value != null);

            Assert.Equal("Aggregate function '' not supported by CounterEvaluator", exception.Message);
        }

        [Theory, AutoData]
        public void Evaluate_SumAggFunction_ExpectedResultReceived(
           CounterData[] counterData,
           string filter)
        {
            // Arrange
            string function = "Sum";
            int period = 5;
            string tag = counterData.First().Tags.First().Key;

            DateTime timestamp = DateTime.UtcNow.AddMinutes(1);

            for (int f = 0; f < counterData.Count(); f++)
            {
                counterData[f].TimeStamp = timestamp;
                for (int i = 0; i < counterData[f].Tags.Count; i++)
                {
                    foreach (KeyValuePair<string, object> t in counterData[f].Tags)
                    {
                        counterData[f].Tags[t.Key] = fixture.Create<decimal>();
                    }
                }
            }

            DateTime from = DateTime.UtcNow.AddMinutes(-1 * period);
            IEnumerable<KeyValuePair<string, object>> query = counterData
                       .Where(t => true && t.TimeStamp > from)
                       .SelectMany(t => t.Tags).ToList();

            IEnumerable<KeyValuePair<string, object>> set = query.Where(i => i.Key == tag && i.Value != null).ToArray();
            decimal expectedResult = set.Sum(t => Convert.ToDecimal(t.Value, CultureInfo.InvariantCulture));

            // Act
            object result = new CounterEvaluator().Evaluate(counterData, function, tag, null, period);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Theory, AutoData]
        public void Evaluate_CountAggFunction_ExpectedResultReceived(
           CounterData[] counterData,
           string filter)
        {
            // Arrange
            string function = "Count";
            int period = 5;
            string tag = counterData.First().Tags.First().Key;

            DateTime timestamp = DateTime.UtcNow.AddMinutes(1);

            for (int f = 0; f < counterData.Count(); f++)
            {
                counterData[f].TimeStamp = timestamp;
                for (int i = 0; i < counterData[f].Tags.Count; i++)
                {
                    foreach (KeyValuePair<string, object> t in counterData[f].Tags)
                    {
                        counterData[f].Tags[t.Key] = fixture.Create<decimal>();
                    }
                }
            }

            DateTime from = DateTime.UtcNow.AddMinutes(-1 * period);
            IEnumerable<KeyValuePair<string, object>> query = counterData
                       .Where(t => true && t.TimeStamp > from)
                       .SelectMany(t => t.Tags).ToList();

            IEnumerable<KeyValuePair<string, object>> set = query.Where(i => i.Key == tag && i.Value != null).ToArray();
            int expectedResult = set.Count();

            // Act
            object result = new CounterEvaluator().Evaluate(counterData, function, tag, null, period);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Theory, AutoData]
        public void Evaluate_CountDistinctAggFunction_ExpectedResultReceived(
           CounterData[] counterData,
           string filter)
        {
            // Arrange
            string function = "CountDistinct";
            int period = 5;
            string tag = counterData.First().Tags.First().Key;

            DateTime timestamp = DateTime.UtcNow.AddMinutes(1);

            for (int f = 0; f < counterData.Count(); f++)
            {
                counterData[f].TimeStamp = timestamp;
                for (int i = 0; i < counterData[f].Tags.Count; i++)
                {
                    foreach (KeyValuePair<string, object> t in counterData[f].Tags)
                    {
                        counterData[f].Tags[t.Key] = fixture.Create<decimal>();
                    }
                }
            }

            DateTime from = DateTime.UtcNow.AddMinutes(-1 * period);
            IEnumerable<KeyValuePair<string, object>> query = counterData
                       .Where(t => true && t.TimeStamp > from)
                       .SelectMany(t => t.Tags).ToList();

            IEnumerable<KeyValuePair<string, object>> set = query.Where(i => i.Key == tag && i.Value != null).ToArray();
            int expectedResult = set.GroupBy(t => t.Value).Count();

            // Act
            object result = new CounterEvaluator().Evaluate(counterData, function, tag, null, period);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Theory, AutoData]
        public void Evaluate_AvgDistinctAggFunction_ExpectedResultReceived(
          CounterData[] counterData,
          string filter)
        {
            // Arrange
            string function = "Avg";
            int period = 5;
            string tag = counterData.First().Tags.First().Key;

            DateTime timestamp = DateTime.UtcNow.AddMinutes(1);

            for (int f = 0; f < counterData.Count(); f++)
            {
                counterData[f].TimeStamp = timestamp;
                for (int i = 0; i < counterData[f].Tags.Count; i++)
                {
                    foreach (KeyValuePair<string, object> t in counterData[f].Tags)
                    {
                        counterData[f].Tags[t.Key] = fixture.Create<decimal>();
                    }
                }
            }

            DateTime from = DateTime.UtcNow.AddMinutes(-1 * period);
            IEnumerable<KeyValuePair<string, object>> query = counterData
                       .Where(t => true && t.TimeStamp > from)
                       .SelectMany(t => t.Tags).ToList();

            IEnumerable<KeyValuePair<string, object>> set = query.Where(i => i.Key == tag && i.Value != null);
            string expectedResult = set.Average(t => Convert.ToDecimal(t.Value, CultureInfo.InvariantCulture)).ToString("0.#####", CultureInfo.CurrentCulture);

            // Act
            object result = new CounterEvaluator().Evaluate(counterData, function, tag, null, period);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Theory, AutoData]
        public void Evaluate_MinDistinctAggFunction_ExpectedResultReceived(
          CounterData[] counterData,
          string filter)
        {
            // Arrange
            string function = "Min";
            int period = 5;
            string tag = counterData.First().Tags.First().Key;

            DateTime timestamp = DateTime.UtcNow.AddMinutes(1);

            for (int f = 0; f < counterData.Count(); f++)
            {
                counterData[f].TimeStamp = timestamp;
                for (int i = 0; i < counterData[f].Tags.Count; i++)
                {
                    foreach (KeyValuePair<string, object> t in counterData[f].Tags)
                    {
                        counterData[f].Tags[t.Key] = fixture.Create<decimal>();
                    }
                }
            }

            DateTime from = DateTime.UtcNow.AddMinutes(-1 * period);
            IEnumerable<KeyValuePair<string, object>> query = counterData
                       .Where(t => true && t.TimeStamp > from)
                       .SelectMany(t => t.Tags).ToList();

            IEnumerable<KeyValuePair<string, object>> set = query.Where(i => i.Key == tag && i.Value != null);
            object expectedResult = set.Min(t => SwapTypes(t.Value));

            // Act
            object result = new CounterEvaluator().Evaluate(counterData, function, tag, null, period);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Theory, AutoData]
        public void Evaluate_MaxDistinctAggFunction_ExpectedResultReceived(
          CounterData[] counterData,
          string filter)
        {
            // Arrange
            string function = "Max";
            int period = 5;
            string tag = counterData.First().Tags.First().Key;

            DateTime timestamp = DateTime.UtcNow.AddMinutes(1);

            for (int f = 0; f < counterData.Count(); f++)
            {
                counterData[f].TimeStamp = timestamp;
                for (int i = 0; i < counterData[f].Tags.Count; i++)
                {
                    foreach (KeyValuePair<string, object> t in counterData[f].Tags)
                    {
                        counterData[f].Tags[t.Key] = fixture.Create<decimal>();
                    }
                }
            }

            DateTime from = DateTime.UtcNow.AddMinutes(-1 * period);
            IEnumerable<KeyValuePair<string, object>> query = counterData
                       .Where(t => true && t.TimeStamp > from)
                       .SelectMany(t => t.Tags).ToList();

            IEnumerable<KeyValuePair<string, object>> set = query.Where(i => i.Key == tag && i.Value != null);
            object expectedResult = set.Max(t => SwapTypes(t.Value));

            // Act
            object result = new CounterEvaluator().Evaluate(counterData, function, tag, null, period);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Theory, AutoData]
        public void Evaluate_FirstDistinctAggFunction_ExpectedResultReceived(
        CounterData[] counterData,
        string filter)
        {
            // Arrange
            string function = "First";
            int period = 5;
            string tag = counterData.First().Tags.First().Key;

            DateTime timestamp = DateTime.UtcNow.AddMinutes(1);

            for (int f = 0; f < counterData.Count(); f++)
            {
                counterData[f].TimeStamp = timestamp;
                for (int i = 0; i < counterData[f].Tags.Count; i++)
                {
                    foreach (KeyValuePair<string, object> t in counterData[f].Tags)
                    {
                        counterData[f].Tags[t.Key] = fixture.Create<decimal>();
                    }
                }
            }

            DateTime from = DateTime.UtcNow.AddMinutes(-1 * period);
            IEnumerable<KeyValuePair<string, object>> query = counterData
                       .Where(t => true && t.TimeStamp > from)
                       .SelectMany(t => t.Tags).ToList();

            IEnumerable<KeyValuePair<string, object>> set = query.Where(i => i.Key == tag && i.Value != null);
            counterData.FirstOrDefault().Tags.TryGetValue(tag, out object expectedResult);

            // Act
            object result = new CounterEvaluator().Evaluate(counterData, function, tag, null, period);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Theory, AutoData]
        public void Evaluate_LastDistinctAggFunction_ExpectedResultReceived(
        CounterData[] counterData,
        string filter)
        {
            // Arrange
            string function = "Last";
            int period = 5;
            string tag = counterData.First().Tags.First().Key;

            DateTime timestamp = DateTime.UtcNow.AddMinutes(1);

            for (int f = 0; f < counterData.Count(); f++)
            {
                counterData[f].TimeStamp = timestamp;
                for (int i = 0; i < counterData[f].Tags.Count; i++)
                {
                    foreach (KeyValuePair<string, object> t in counterData[f].Tags)
                    {
                        counterData[f].Tags[t.Key] = fixture.Create<decimal>();
                    }
                }
            }

            DateTime from = DateTime.UtcNow.AddMinutes(-1 * period);
            IEnumerable<KeyValuePair<string, object>> query = counterData
                       .Where(t => true && t.TimeStamp > from)
                       .SelectMany(t => t.Tags).ToList();

            IEnumerable<KeyValuePair<string, object>> set = query.Where(i => i.Key == tag && i.Value != null);
            counterData.LastOrDefault().Tags.TryGetValue(tag, out object expectedResult);

            // Act
            object result = new CounterEvaluator().Evaluate(counterData, function, tag, null, period);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Theory, AutoData]
        public void Evaluate_FirstNotEmptyDistinctAggFunction_ExpectedResultReceived(
          CounterData[] counterData,
          string filter)
        {
            // Arrange
            string function = "FirstNotEmpty";
            int period = 5;
            string tag = counterData.First().Tags.First().Key;

            DateTime timestamp = DateTime.UtcNow.AddMinutes(1);

            for (int f = 0; f < counterData.Count(); f++)
            {
                counterData[f].TimeStamp = timestamp;
                for (int i = 0; i < counterData[f].Tags.Count; i++)
                {
                    foreach (KeyValuePair<string, object> t in counterData[f].Tags)
                    {
                        counterData[f].Tags[t.Key] = fixture.Create<decimal>();
                    }
                }
            }

            DateTime from = DateTime.UtcNow.AddMinutes(-1 * period);
            IEnumerable<KeyValuePair<string, object>> query = counterData
                       .Where(t => true && t.TimeStamp > from)
                       .SelectMany(t => t.Tags).ToList();

            IEnumerable<KeyValuePair<string, object>> set = query.Where(i => i.Key == tag && i.Value != null);
            object expectedResult = set.FirstOrDefault(t => t.Value != null).Value;

            // Act
            object result = new CounterEvaluator().Evaluate(counterData, function, tag, null, period);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Theory, AutoData]
        public void Evaluate_LastNotEmptyDistinctAggFunction_ExpectedResultReceived(
          CounterData[] counterData,
          string filter)
        {
            // Arrange
            string function = "LastNotEmpty";
            int period = 5;
            string tag = counterData.First().Tags.First().Key;

            DateTime timestamp = DateTime.UtcNow.AddMinutes(1);

            for (int f = 0; f < counterData.Count(); f++)
            {
                counterData[f].TimeStamp = timestamp;
                for (int i = 0; i < counterData[f].Tags.Count; i++)
                {
                    foreach (KeyValuePair<string, object> t in counterData[f].Tags)
                    {
                        counterData[f].Tags[t.Key] = fixture.Create<decimal>();
                    }
                }
            }

            DateTime from = DateTime.UtcNow.AddMinutes(-1 * period);
            IEnumerable<KeyValuePair<string, object>> query = counterData
                       .Where(t => true && t.TimeStamp > from)
                       .SelectMany(t => t.Tags).ToList();

            IEnumerable<KeyValuePair<string, object>> set = query.Where(i => i.Key == tag && i.Value != null);
            object expectedResult = set.LastOrDefault(t => t.Value != null).Value;

            // Act
            object result = new CounterEvaluator().Evaluate(counterData, function, tag, null, period);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        private object SwapTypes(object value)
        {
            if (decimal.TryParse(value.ToString(), out decimal resultDecimal))
            {
                return resultDecimal;
            }
            else if (DateTime.TryParse(value.ToString(), out DateTime resultDate))
            {
                return resultDate;
            }
            else
            {
                return value;
            }
        }

        /*
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
        }*/
    }
}
