using AutoFixture.Xunit2;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using WorkflowEngine.Core;
using WorkflowEngine.Core.Dependencies;
using WorkflowEngine.Core.Dependencies.Lists;
using WorkflowEngine.Core.Evaluation;
using WorkflowEngine.Misc;
using Xunit;

namespace WorkflowEngine.Test.CoreTests.EvaluationTest
{
    public class ParametersTest
    {
        #region SetParameter
        [Fact]
        public void SetParameter_SeNewParameter_Successfully()
        {
            // Arrange
            Parameters parameters = GetParametersStub();
            string name = "nemeTest";
            string value = "valueTest";

            // Act
            parameters.SetParameter(name, value);

            // Assert
            Parameter parameter = parameters.FirstOrDefault(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));
            Assert.Equal(value, parameter?.Value);
        }

        [Fact]
        public void SetParameter_OverwriteParameter_Successfully()
        {
            // Arrange
            Parameters parameters = GetParametersStub();
            string name = "nemeTest1";
            string value = "valueTest";

            // Act
            parameters.SetParameter(name, value);

            // Assert
            Parameter parameter = parameters.FirstOrDefault(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));
            Assert.Equal(value, parameter?.Value);
        }
        #endregion

        #region GetParameter

        [Fact]
        public void GetParameter_GetNonExistentElement_ReturnNull()
        {
            // Arrange
            Parameters parameters = GetParametersStub();

            // Act
            Parameter parameter = parameters.GetParameter("nemeTest");

            // Assert
            Assert.Null(parameter);
        }

        [Fact]
        public void GetParameter_GetExistentElement_ReturnElement()
        {
            // Arrange
            Parameters parameters = GetParametersStub();

            // Act
            Parameter parameter = parameters.GetParameter("nemeTest1");

            // Assert
            Assert.NotNull(parameter);
        }
        #endregion

        #region GetArrayOfValues

        [Fact]
        public void GetArrayOfValues_ParseParametersToArray_Successfullyt()
        {
            // Arrange
            Parameters parameters = GetParametersStub();
            string[] expectedArray = new[] { "valueTest1", null, "valueTest3", "4" };

            // Act
            string[] parameter = parameters.GetArrayOfValues();

            // Assert
            Assert.Equal(expectedArray, parameter);
        }
        #endregion

        #region GetParameter

        [Fact]
        public void GetParameter_ParameterNotFound_ThrowWorkflowException()
        {
            // Arrange
            Parameters parameters = GetParametersStub();

            // Act
            WorkflowException workflowException = Assert.Throws<WorkflowException>(() => parameters.GetParameter<int>("nemeTest"));

            // Assert
            Assert.Equal("Parameter 'nemeTest' was not found", workflowException.Message);
        }

        [Fact]
        public void GetParameter_ReturnParameterWithDefaulValue_Successfully()
        {
            // Arrange
            Parameters parameters = GetParametersStub();

            // Act
            string result = parameters.GetParameter<string>("nemeTest2");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetParameter_ReturnParameterAndConvertToString_Successfully()
        {
            // Arrange
            Parameters parameters = GetParametersStub();

            // Act
            string result = parameters.GetParameter<string>("nemeTest1");

            // Assert
            Assert.Equal("valueTest1", result);
        }
        #endregion

        #region GetParameterOrDefault

        [Fact]
        public void GetParameterOrDefault_ReturnParameter_Successfully()
        {
            // Arrange
            Parameters parameters = GetParametersStub();

            // Act
            string result = parameters.GetParameterOrDefault<string>("nemeTest1");

            // Assert
            Assert.Equal("valueTest1", result);
        }

        [Fact]
        public void GetParameterOrDefault_ReturnIntParameter_Return4()
        {
            // Arrange
            Parameters parameters = GetParametersStub();

            // Act
            int result = parameters.GetParameterOrDefault<int>("nemeTest4");

            // Assert
            Assert.Equal(4, result);
        }

        [Fact]
        public void GetParameterOrDefault_ReturnNullableParameter_Return4()
        {
            // Arrange
            Parameters parameters = GetParametersStub();

            // Act
            int? result = parameters.GetParameterOrDefault<int?>("nemeTest4");

            // Assert
            Assert.Equal(4, result);
        }

        [Fact]
        public void GetParameterOrDefault_ReturnNullableParameter_ReturnNull()
        {
            // Arrange
            Parameters parameters = GetParametersStub();

            // Act
            int? result = parameters.GetParameterOrDefault<int?>("nemeTest2");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetParameterOrDefault_ReturnDefaultValueForNullable_ReturnNull()
        {
            // Arrange
            Parameters parameters = GetParametersStub();

            // Act
            int? result = parameters.GetParameterOrDefault<int?>("nemeTest123");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetParameterOrDefault_ReturnDefaultValueForInt_Return0()
        {
            // Arrange
            Parameters parameters = GetParametersStub();

            // Act
            int result = parameters.GetParameterOrDefault<int>("nemeTest123");

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void GetParameterOrDefault_ReturnDefault_Successfully()
        {
            // Arrange
            Parameters parameters = GetParametersStub();

            // Act
            string result = parameters.GetParameterOrDefault("nemeTest", "valueTest");

            // Assert
            Assert.Equal("valueTest", result);
        }
        #endregion

        #region Read

        [Theory, AutoData]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1305:Укажите IFormatProvider", Justification = "<Ожидание>")]
        public void ReadParameters_OnAllDataCorrect_ExpectedResultReceived((string, string)[] values, Mock<IInstance> mockInstance)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<Workflow>");
            for (int i = 0; i < values.Length; i++)
            {
                builder = builder.AppendLine($"<parameter type=\"appData\" name=\"{values[i].Item1}\" value=\"{values[i].Item2}\" />");
            }

            builder = builder.AppendLine($"<parameter type=\"Settings\" name=\"testSettings\" value=\"FxConnector_Result.AmountToEuro\" default = \"unknown\" />");
            builder = builder.AppendLine($"<parameter type=\"List\" name=\"testList\" value=\"valueList\" options=\"ToLowerCase\" />");
            builder = builder.AppendLine($"<parameter name=\"testParameter\" value=\"valueTestParameter\" options=\"ToUpperCase\" />");
            builder = builder.AppendLine($"<parameter type = \"random\" name=\"random1\" value = \"int:1-100\" />");
            builder = builder.AppendLine($"<parameter type=\"appData\" name=\"split\" value=\"test1|test2\" />");
            builder = builder.AppendLine($"<parameter name=\"testList\" value=\"testList\" options=\"list\" />");
            builder.AppendLine("</Workflow>");
            string xml = builder.ToString();

            Mock<IDependencyContainer> mockDependencyContainer = new Mock<IDependencyContainer>();
            Mock<IServiceProvider> mockServiceProvider = new Mock<IServiceProvider>();
            Mock<IListService> listService = new Mock<IListService>();

            listService.Setup(x => x.GetList(It.Is<string>(v => v == "valueList"))).Returns(new List<ListItem>()
            {
                new ListItem()
                {
                    Id = 1,
                    Name = "Name",
                    ExtendedData = new Dictionary<string, string>()
                }
            });

            mockServiceProvider.Setup(x => x.GetService(typeof(IListService))).Returns(listService.Object);
            mockDependencyContainer.Setup(x => x.ServiceProvider).Returns(mockServiceProvider.Object);
            Instance instance = new Instance()
            {
                DC = mockDependencyContainer.Object
            };

            Parameters parameters = new Parameters().Read(XElement.Parse(xml), instance);

            // Assert
            Assert.Equal(8, parameters.Count);

            for (int i = 0; i < 3; i++)
            {
                Assert.Equal(parameters[i].Name, values[i].Item1);
            }

            Assert.Equal(parameters[3].Name, "testSettings");
            Assert.Equal(parameters[3].Value, "unknown");
            Assert.Equal(parameters[4].Name, "testList");
            Assert.Equal(parameters[4].Value, "name");
            Assert.Equal(parameters[5].Name, "testParameter");
            Assert.Equal(parameters[5].Value, "VALUETESTPARAMETER");
            Assert.Equal(parameters[6].Name, "random1");

            int randomInt = int.Parse(parameters[6].Value.ToString());
            Assert.True(randomInt > 0 && randomInt < 100);
        }

        [Theory, AutoData]
        public void ReadParameters_List_ExpectedResultReceived(
                                                  Mock<IDependencyContainer> mockDependencyContainer,
                                                  Mock<IContextData> mockContextData,
                                                  Mock<IInstance> mockInstance,
                                                  Mock<IListService> mockListService)
        {
            // Arrange
            string xml = "<Workflow><parameter name=\"testList\" value=\"testlistval\" options=\"list\" /></Workflow>";
            JObject expected = JObject.Parse("{\"testListVal\":{\"listitem\":{\"listitem1\":\"EP221241\"}}}");
            IList<ListItem> listItems = new List<ListItem>
            {
              new ListItem { Id = 1, Name = "ListItem1" }
            };

            mockContextData.Setup(p => p.Data)
                           .Returns(expected);
            mockListService.Setup(m => m.GetList(It.IsAny<string>()))
                           .Returns(listItems);
            mockInstance.Setup(p => p.ContextData)
                        .Returns(mockContextData.Object);
            mockInstance.Setup(p => p.DC)
                        .Returns(mockDependencyContainer.Object);
            mockInstance.Setup(p => p.GetDependency<IListService>())
                        .Returns(mockListService.Object);

            // Act
            Parameters parameters = new Parameters().Read(XElement.Parse(xml), mockInstance.Object);

            // Assert
            mockListService.Verify(m => m.GetList(It.Is<string>(v => v == "testList")));

            Assert.Equal(1, parameters.Count);
            Assert.Equal(parameters[0].Name, "listitem1");
            Assert.Equal(parameters[0].Value, "ep221241");
        }

        [Theory, AutoData]
        public void ReadParameters_ListToKeyValue_ExpectedResultReceived(
                                                 Mock<IDependencyContainer> mockDependencyContainer,
                                                 Mock<IContextData> mockContextData,
                                                 Mock<IInstance> mockInstance,
                                                 Mock<IListService> mockListService)
        {
            // Arrange
            string xml = "<Workflow><parameter name=\"testList2\" value=\"testlistval2\" options=\"listToKeyValue\" /></Workflow>";
            JObject expected = JObject.Parse("{\"testlistval2\":{\"listitem\":{\"listitem1\":\"EP221242\"}}}");
            IList<ListItem> listItems = new List<ListItem>
            {
              new ListItem { Id = 1, Name = "ListItem1" }
            };

            mockContextData.Setup(p => p.Data)
                           .Returns(expected);
            mockListService.Setup(m => m.GetList(It.IsAny<string>()))
                           .Returns(listItems);
            mockInstance.Setup(p => p.ContextData)
                        .Returns(mockContextData.Object);
            mockInstance.Setup(p => p.DC)
                        .Returns(mockDependencyContainer.Object);
            mockInstance.Setup(p => p.GetDependency<IListService>())
                        .Returns(mockListService.Object);

            // Act
            Parameters parameters = new Parameters().Read(XElement.Parse(xml), mockInstance.Object);

            // Assert
            mockListService.Verify(m => m.GetList(It.Is<string>(v => v == "testList2")));

            Assert.Equal(2, parameters.Count);
            Assert.Equal(parameters[0].Name, "key");
            Assert.Equal(parameters[0].Value, "listitem1");
            Assert.Equal(parameters[1].Name, "value");
            Assert.Equal(parameters[1].Value, "ep221242");
        }

        [Theory]
        [InlineData("Recurring.CutOffAmount", "8")]
        [InlineData("Recurring", "7")]
        public void ReadParameters_Settings_ExpectedResultReceived(string path, string val)
        {
            // Arrange
            string xml = $"<Workflow><parameter type=\"settings\" value=\"{path}\" /></Workflow>";
            IList<ListItem> listItems = new List<ListItem>
            {
              new ListItem { Id = 1, Name = "Recurring", ExtendedData = new Dictionary<string, string> { { "Random", "7" }, { "CutOffAmount", "8" } } }
            };

            Mock<IDependencyContainer> mockDependencyContainer = new Mock<IDependencyContainer>();
            Mock<IInstance> mockInstance = new Mock<IInstance>();
            Mock<IListService> mockListService = new Mock<IListService>();

            mockListService.Setup(m => m.GetList(It.IsAny<string>()))
                           .Returns(listItems);
            mockInstance.Setup(p => p.DC)
                        .Returns(mockDependencyContainer.Object);
            mockInstance.Setup(p => p.GetDependency<IListService>())
                        .Returns(mockListService.Object);

            // Act
            Parameters parameters = new Parameters().Read(XElement.Parse(xml), mockInstance.Object);

            // Assert
            mockListService.Verify(m => m.GetList(It.Is<string>(v => v == "Settings")));

            Assert.Equal(1, parameters.Count);
            Assert.Equal(parameters[0].Name, "parameter_0");
            Assert.Equal(parameters[0].Value, val);
        }
        #endregion

        #region Stubs
        private static Parameters GetParametersStub()
        {
            Parameters strategyContextData = new Parameters
            {
                new Parameter { Name = "nemeTest1", Value = "valueTest1" },
                new Parameter { Name = "nemeTest2", Value = null },
                new Parameter { Name = "nemeTest3", Value = "valueTest3" },
                new Parameter { Name = "nemeTest4", Value = 4 }
            };

            return strategyContextData;
        }

        #endregion
    }
}
