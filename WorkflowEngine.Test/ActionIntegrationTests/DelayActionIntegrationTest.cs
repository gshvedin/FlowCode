using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using System.Threading;

namespace WorkflowEngine.Test.ActionIntegrationTests
{
    public class DelayActionIntegrationTest
    {
        private readonly ITestOutputHelper testOutputHelper;

        public DelayActionIntegrationTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task DelayAction_Delay1000ms()
        {
            // Arrange
            Stopwatch timer = new Stopwatch();
            Workflow workFlow = new Workflow(GetXmlDelay1000Stub(), DC.GetInstance());
            workFlow.CurrentInstance.MaxDegreeOfParallelism = 25;

            // Act
            timer.Start();
            string executionResult = await workFlow.ExecuteAsync("{}", CancellationToken.None);
            timer.Stop();

            long time = timer.ElapsedMilliseconds;
            testOutputHelper.WriteLine(string.Format(CultureInfo.CurrentCulture, "Execution time: {0}", time));

            // Assert
            Assert.Equal("{\n  \"StartProcess\": null\n}".ReplaceLineEndings(), executionResult.ReplaceLineEndings());
            Assert.True(1000 < time && time < 1100);
        }

        [Fact]
        public async Task DelayAction_DelayDefault500ms()
        {
            // Arrange
            Stopwatch timer = new Stopwatch();
            Workflow workFlow = new Workflow(GetXmlDelayDefaultStub(), DC.GetInstance());
            workFlow.CurrentInstance.MaxDegreeOfParallelism = 25;

            // Act
            timer.Start();
            string executionResult = await workFlow.ExecuteAsync("{}", CancellationToken.None);
            timer.Stop();

            long time = timer.ElapsedMilliseconds;
            testOutputHelper.WriteLine(string.Format(CultureInfo.CurrentCulture, "Execution time: {0}", time));

            // Assert
            Assert.True(500 < time && time < 600);
        }

        #region Stub
        private static string GetXmlDelay1000Stub()
        {
            StringBuilder xmlStringBuilder = new StringBuilder();
            xmlStringBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            xmlStringBuilder.Append("<Workflow>");
            xmlStringBuilder.Append("<Delay name=\"WaitForDelay500\" ms=\"1000\" />");
            xmlStringBuilder.Append("</Workflow>");

            return xmlStringBuilder.ToString();
        }

        private static string GetXmlDelayDefaultStub()
        {
            StringBuilder xmlStringBuilder = new StringBuilder();
            xmlStringBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            xmlStringBuilder.Append("<Workflow>");
            xmlStringBuilder.Append("<Delay name=\"WaitForDelay500\" ms=\"qwert\" />");
            xmlStringBuilder.Append("</Workflow>");

            return xmlStringBuilder.ToString();
        }
        #endregion
    }
}