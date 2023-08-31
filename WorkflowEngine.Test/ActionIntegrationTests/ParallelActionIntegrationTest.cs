using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace WorkflowEngine.Test.ActionIntegrationTests
{
    public class ParallelActionIntegrationTest
    {
        private readonly ITestOutputHelper testOutputHelper;

        public ParallelActionIntegrationTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task ParallelAction_Delay1100ms()
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
            Assert.True(1100 < time && time < 1200);
        }

        #region Stub
        private static string GetXmlDelay1000Stub()
        {
            StringBuilder xmlStringBuilder = new StringBuilder();
            xmlStringBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            xmlStringBuilder.Append("<Workflow>");
            xmlStringBuilder.Append("  <Parallel>");
            xmlStringBuilder.Append("    <Delay name=\"WaitForDelay500\" ms=\"1000\" />");
            xmlStringBuilder.Append("    <Delay name=\"WaitForDelay500\" ms=\"800\" />");
            xmlStringBuilder.Append("    <Delay name=\"WaitForDelay500\" ms=\"1100\" />");
            xmlStringBuilder.Append("  </Parallel>");
            xmlStringBuilder.Append("</Workflow>");

            return xmlStringBuilder.ToString();
        }
        #endregion
    }
}
