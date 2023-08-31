using AutoFixture.Xunit2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WorkflowEngine.Helpers;
using Xunit;

namespace WorkflowEngine.Test.HelpersTest
{
    public class JsonExtensionsTest
    {
        [Theory, AutoData]
        public void GetAttribute_GetValueByAttributeName_Successful(string val, string testVal)
        {
            // Arrange
            JObject obj = JObject.Parse(JsonConvert.SerializeObject(new { oldVal = new { oldVal = val } }));
            JObject expectedResult = JObject.Parse(JsonConvert.SerializeObject(new { oldVal = new { oldVal = val }, test = new { testVal = testVal } }));

            // Act
            JObject result = obj.TryAddByPath("test", JToken.Parse(JsonConvert.SerializeObject(new { testVal = testVal })));

            // Assert
            Assert.Equal(obj, result);
        }
    }
}
