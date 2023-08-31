using AutoFixture.Xunit2;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using WorkflowEngine.Actions.Implementations;
using WorkflowEngine.Core;
using WorkflowEngine.Core.Dependencies;
using WorkflowEngine.Core.Dependencies.Strategies;
using Xunit;

namespace WorkflowEngine.Test.CoreTests
{
    public class ContextDataTest
    {
        #region Constructor
        [Theory, AutoData]
        public void Constructor_InitializeData_Successfully(Mock<IInstance> mockInstance)
        {
            // Arrange

            // Act
            ContextData contextData = new ContextData(GetContextDataStub(), mockInstance.Object);

            // Assert
            Assert.NotEqual(default(JObject), contextData.Data);
            Assert.Equal("testValue", contextData.Data.SelectToken("testName").ToString());
        }

        #endregion

        [Theory, AutoData]
        public void CurrentStrategyContext_SetValue_Successfully(
                                                      Mock<IInstance> mockInstance,
                                                      Mock<IStrategyContext> mockStrategyContext)
        {
            // Arrange
            ContextData contextData = new ContextData(GetContextDataStub(), mockInstance.Object)
            {
                // Act
                CurrentStrategyContext = mockStrategyContext.Object
            };

            // Assert
            Assert.NotEqual(default(JObject), contextData.Data);
            Assert.Equal(mockStrategyContext.Object, contextData.CurrentStrategyContext);
        }

        [Theory, AutoData]
        public void BreakProcess_SetValue_Successfully(Mock<IInstance> mockInstance)
        {
            // Arrange
            ContextData contextData = new ContextData(GetContextDataStub(), mockInstance.Object)
            {
                // Act
                BreakProcess = true
            };

            // Assert
            Assert.NotEqual(default(JObject), contextData.Data);
            Assert.True(contextData.BreakProcess);
        }

        [Theory, AutoData]
        public void GoToAction_SetValue_Successfully(
                                                   Mock<IInstance> mockInstance,
                                                   string goToActionValue)
        {
            // Arrange
            ContextData contextData = new ContextData(GetContextDataStub(), mockInstance.Object)
            {
                // Act
                GoToAction = goToActionValue
            };

            // Assert
            Assert.NotEqual(default(JObject), contextData.Data);
            Assert.Equal(goToActionValue, contextData.GoToAction);
        }

        #region SetValue
        [Theory, AutoData]
        public void SetValue_SetNewValue_Successfully(string testName, string testValue, Mock<IInstance> mockInstance)
        {
            // Arrange
            ContextData contextData = new ContextData(GetContextDataStub(), mockInstance.Object);

            // Act
            contextData.SetValue(testName, testValue);

            // Assert
            Assert.NotEqual(default(JObject), contextData.Data);
            Assert.Equal(testValue, contextData.Data.SelectToken(testName).ToString());
        }

        [Theory, AutoData]
        public void SetValue_SetNullValue_Successfully(string testName, Mock<IInstance> mockInstance)
        {
            // Arrange
            ContextData contextData = new ContextData(GetContextDataStub(), mockInstance.Object);

            // Act
            contextData.SetValue(testName, null);

            // Assert
            Assert.NotEqual(default(JObject), contextData.Data);
            Assert.Equal(string.Empty, contextData.Data.SelectToken(testName).ToString());
        }

        [Theory, AutoData]
        public void SetValue_ReplaceExistingValue_Successfully(Mock<IInstance> mockInstance)
        {
            // Arrange
            string testName = "testName";
            string testValue = "testValue2";
            ContextData contextData = new ContextData(GetContextDataStub(), mockInstance.Object);

            // Act
            contextData.SetValue(testName, testValue);

            // Assert
            Assert.NotEqual(default(JObject), contextData.Data);
            Assert.Equal(testValue, contextData.Data.SelectToken(testName).ToString());
        }

        [Theory, AutoData]
        public void SetValue_ReplaceExistingValueNull_Successfully(Mock<IInstance> mockInstance)
        {
            // Arrange
            string testName = "testName";
            ContextData contextData = new ContextData(GetContextDataStub(), mockInstance.Object);

            // Act
            contextData.SetValue(testName, null);

            // Assert
            Assert.NotEqual(default(JObject), contextData.Data);
            Assert.Equal(string.Empty, contextData.Data.SelectToken(testName).ToString());
        }
        #endregion

        #region SetValueAsJsonNode

        [Theory, AutoData]
        public void SetValueAsJsonNode_SetNewValue_Successfully(string testName, Mock<IInstance> mockInstance)
        {
            // Arrange
            ContextData contextData = new ContextData(GetContextDataStub(), mockInstance.Object);

            // Act
            contextData.SetValueAsJsonNode(testName, GetContextDataStub());

            // Assert
            Assert.NotEqual(default(JObject), contextData.Data);
            Assert.Equal(
                GetContextDataStub().Replace("\r", string.Empty, StringComparison.CurrentCulture),
                contextData.Data.SelectToken(testName).ToString().Replace("\r", string.Empty, StringComparison.CurrentCulture));
        }

        [Theory, AutoData]
        public void SetValueAsJsonNode_ReplaceExistingValue_Successfully(Mock<IInstance> mockInstance)
        {
            // Arrange
            string testName = "testName";
            ContextData contextData = new ContextData(GetContextDataStub(), mockInstance.Object);

            // Act
            contextData.SetValueAsJsonNode(testName, GetContextDataStub2());

            // Assert
            Assert.NotEqual(default(JObject), contextData.Data);
            Assert.Equal(
                GetContextDataStub2().Replace("\r", string.Empty, StringComparison.CurrentCulture),
                contextData.Data.SelectToken(testName).ToString().Replace("\r", string.Empty, StringComparison.CurrentCulture));
        }

        [Theory, AutoData]
        public void SetValueAsJsonNode_SetValueFromItems_Successfully(Mock<IInstance> mockInstance)
        {
            // Arrange
            string testName = "testName2";
            ContextData contextData = new ContextData(GetContextDataStub(), mockInstance.Object);

            // Act
            contextData.SetValueAsJsonNode(testName, GetContextDataWithItemsStub());

            // Assert
            Assert.NotEqual(default(JObject), contextData.Data);
            Assert.Equal("ItemsValue", contextData.Data.SelectToken(testName).ToString());
        }

        [Theory, AutoData]
        public void SetValueAsJsonNode_SetValueArray_Successfully(string testName, Mock<IInstance> mockInstance)
        {
            // Arrange
            ContextData contextData = new ContextData(GetContextDataStub(), mockInstance.Object);

            // Act
            contextData.SetValueAsJsonNode(testName, GetContextDataArrayStub());

            // Assert
            Assert.NotEqual(default(JObject), contextData.Data);
            Assert.Equal(
                GetContextDataArrayStub().Replace("\r", string.Empty, StringComparison.CurrentCulture),
                contextData.Data.SelectToken(testName).ToString().Replace("\r", string.Empty, StringComparison.CurrentCulture));
        }

        [Theory, AutoData]
        public void SetValueAsJsonNode_SetNullValue_(string testName, Mock<IInstance> mockInstance)
        {
            // Arrange
            ContextData contextData = new ContextData(GetContextDataStub(), mockInstance.Object);

            // Act
            Exception exception = Record.Exception(() => contextData.SetValueAsJsonNode(testName, null));

            // Assert
            Assert.Equal("Nullable object must have a value.", exception.Message);
        }
        #endregion

        #region GetValue
        [Theory, AutoData]
        public void GetValue_GetExistingValue_Successfully(Mock<IInstance> mockInstance)
        {
            // Arrange
            ContextData contextData = new ContextData(GetContextDataStub(), mockInstance.Object);

            // Act
            string val = contextData.GetValue("testName");

            // Assert
            Assert.Equal("testValue", val);
        }

        [Theory, AutoData]
        public void GetValue_GetNotExistingValue_ReturnNull(Mock<IInstance> mockInstance)
        {
            // Arrange
            ContextData contextData = new ContextData(GetContextDataStub(), mockInstance.Object);

            // Act
            string val = contextData.GetValue("testName1");

            // Assert
            Assert.Null(val);
        }

        [Theory, AutoData]
        public void GetValue_GetFromArrayProp_Successfully(Mock<IInstance> mockInstance)
        {
            // Arrange
            ContextData contextData = new ContextData(GetObjWithArrayStub(), mockInstance.Object);

            // Act
            string val = contextData.GetValue("documents[?(@.type == 'Passport')].number");

            // Assert
            Assert.Equal(val, "EP221241");
        }

        [Theory]
        [InlineData("count_success_t_user_1h", "1")]
        [InlineData("count_fails_t_user_1h", "2")]
        [InlineData("count_risk_declines_user_1h", "3")]
        [InlineData("count_risk_declines_user_1d", "4")]
        public void GetValue_GetFromDictionary_Successfully(string key, string expected)
        {
            // Arrange
            Mock<IInstance> mockInstance = new Mock<IInstance>();
            ContextData contextData = new ContextData(GetDictionaryStub(), mockInstance.Object);

            // Act
            string val = contextData.GetValue($"BlacklistStrategy_Result.{key}");

            // Assert
            Assert.Equal(expected, val);
        }

        #endregion

        #region GetCurrentProcess
        [Theory, AutoData]
        public void GetCurrentProcess_GetValue_Successfully(Mock<IInstance> mockInstance)
        {
            // Arrange
            string contextDataStr = "{\r\n  \"CurrentProcess\": \"testProces\"\r\n}";
            ContextData contextData = new ContextData(contextDataStr, mockInstance.Object);

            // Act
            string val = contextData.GetCurrentProcess();

            // Assert
            Assert.Equal("testProces", val);
        }

        #endregion

        #region GetCurrentRequestId
        [Theory, AutoData]
        public void GetCurrentGuidRequestId_GetRequestId_Successfully(
                                           Guid requestId,
                                           Mock<IDependencyContainer> mockDependencyContainer,
                                           Mock<IInstance> mockInstance)
        {
            // Arrange
            Dictionary<string, object> dictionaryTest = new Dictionary<string, object>
            {
                { "RequestId", requestId }
            };

            mockInstance.SetupGet(v => v.DC)
                        .Returns(mockDependencyContainer.Object);
            mockDependencyContainer.SetupGet(v => v.MetaInfo)
                                   .Returns(dictionaryTest);

            ContextData contextData = new ContextData("{}", mockInstance.Object);

            // Act
            Guid id = contextData.GetCurrentRequestId();

            // Assert
            Assert.Equal(requestId, id);
        }

        [Theory, AutoData]
        public void GetCurrentStringRequestId_GetRequestId_Successfully(
                                          Mock<IDependencyContainer> mockDependencyContainer,
                                          Mock<IInstance> mockInstance)
        {
            // Arrange
            string requestId = "c3421cdd-b9ac-48e5-b306-48b836c05223";
            Dictionary<string, object> dictionaryTest = new Dictionary<string, object>
            {
                { "RequestId",  requestId }
            };

            mockInstance.SetupGet(v => v.DC)
                        .Returns(mockDependencyContainer.Object);
            mockDependencyContainer.SetupGet(v => v.MetaInfo)
                                   .Returns(dictionaryTest);

            ContextData contextData = new ContextData("{}", mockInstance.Object);

            // Act
            Guid id = contextData.GetCurrentRequestId();

            // Assert
            Assert.Equal(requestId, id.ToString());
        }

        [Theory, AutoData]
        public void GetCurrentStringRequestId_GetDefaultId_ReturnGuidEmpty(
                                        Mock<IDependencyContainer> mockDependencyContainer,
                                        Mock<IInstance> mockInstance)
        {
            // Arrange
            mockInstance.SetupGet(v => v.DC)
                        .Returns(mockDependencyContainer.Object);
            mockDependencyContainer.SetupGet(v => v.MetaInfo)
                                   .Returns(new Dictionary<string, object>());

            ContextData contextData = new ContextData("{}", mockInstance.Object);

            // Act
            Guid id = contextData.GetCurrentRequestId();

            // Assert
            Assert.Equal(Guid.Empty, id);
        }

        [Fact]
        public void GetCurrentStringRequestId_SetNullInstance_ReturnGuidEmpty()
        {
            // Arrange
            ContextData contextData = new ContextData("{}", null);

            // Act
            Guid id = contextData.GetCurrentRequestId();

            // Assert
            Assert.Equal(Guid.Empty, id);
        }

        [Theory, AutoData]
        public void GetCurrentStringRequestId_SetNullDependencyContainer_ReturnGuidEmpty(Mock<IInstance> mockInstance)
        {
            // Arrange
            mockInstance.SetupGet(v => v.DC)
                      .Returns(default(IDependencyContainer));

            ContextData contextData = new ContextData("{}", mockInstance.Object);

            // Act
            Guid id = contextData.GetCurrentRequestId();

            // Assert
            Assert.Equal(Guid.Empty, id);
        }

        [Theory, AutoData]
        public void GetCurrentStringRequestId_SetNullMetaInfo_ReturnGuidEmpty(
                                        Mock<IDependencyContainer> mockDependencyContainer,
                                        Mock<IInstance> mockInstance)
        {
            // Arrange
            mockDependencyContainer.SetupGet(v => v.MetaInfo)
                                   .Returns(default(Dictionary<string, object>));
            mockInstance.SetupGet(v => v.DC)
                       .Returns(mockDependencyContainer.Object);

            ContextData contextData = new ContextData("{}", mockInstance.Object);

            // Act
            Guid id = contextData.GetCurrentRequestId();

            // Assert
            Assert.Equal(Guid.Empty, id);
        }
        #endregion

        #region SetCurrentProcess
        [Theory, AutoData]
        public void SetCurrentProcess_SetInExecutedActionsAndData_Successfully(XElement item, Mock<IInstance> mockInstance)
        {
            // Arrange
            StartProcess startProcess = new StartProcess(item);
            ContextData contextData = new ContextData(GetContextDataStub(), mockInstance.Object);

            // Act
            contextData.SetCurrentProcess(startProcess);

            // Assert
            Assert.True(contextData.IsInitialized);
            Assert.Equal("StartProcess", contextData.GetValue("CurrentProcess"));
        }
        #endregion

        #region CompressedData
        [Theory, AutoData]
        public void GetCompressedData_WorkflowCorrect_ReturnCorrectJObject(Mock<IInstance> mockInstance, Mock<IServiceProvider> mockServiceProvider, Mock<IDependencyContainer> dependencyContainer)
        {
            string workflow = GetWorkflowDefinition(); // "<Workflow>\r\n  <!--START-->\r\n  <Start />\r\n \r\n  <Finish />\r\n</Workflow>";
            JObject expected = JObject.Parse("{\r\n  \"documents\": {\r\n    \"number\": \"EP221241\"\r\n  },\r\n  \"phone\": \"+380675163013\"\r\n}");
            mockInstance.Setup(x => x.WorkflowDefinition).Returns(workflow);

            Mock<IStrategyService> strategyService = new Mock<IStrategyService>();
            strategyService.Setup(x => x.GetStrategyDefinitionAsync(It.Is<string>(v => v == "BlacklistStrategy"))).ReturnsAsync(GetBlacklistStrategyDefinition());
            strategyService.Setup(x => x.GetStrategyDefinitionAsync(It.Is<string>(v => v == "WithdrawalConnectorStrategy"))).ReturnsAsync(GetWithdrawalConnectorStrategyDefinition());

            mockServiceProvider.Setup(x => x.GetService(typeof(IStrategyService))).Returns(strategyService.Object);
            dependencyContainer.SetupGet(x => x.ServiceProvider).Returns(mockServiceProvider.Object);

            mockInstance.Setup(x => x.DC).Returns(dependencyContainer.Object);
            Instance instance = new Instance()
            {
                DC = dependencyContainer.Object,
                WorkflowDefinition = workflow
            };

            ContextData contextData = new ContextData(GetObjWithArrayStub(), instance);
            var service = contextData.CurrentInstance.GetDependency<IStrategyService>();
            JObject compressed = contextData.CompressedData;
            string str = compressed.ToString();

            Assert.Equal(expected, compressed);
        }
        #endregion

        #region Stubs
        private static string GetContextDataStub()
        {
            return "{\r\n  \"testName\": \"testValue\"\r\n}";
        }

        private static string GetContextDataStub2()
        {
            return "{\r\n  \"testName2\": \"testValue2\"\r\n}";
        }

        private static string GetContextDataWithItemsStub()
        {
            return "{\r\n  \"Items\": \"ItemsValue\"\r\n}";
        }

        private static string GetContextDataArrayStub()
        {
            return "[\r\n  \"testName\",\r\n  \"testValue\"\r\n]";
        }

        private static string GetDictionaryStub()
        {
            return "{" +
                      "BlacklistStrategy_Result : {" +
                         "\"count_success_t_user_1h\":1," +
                         "\"count_fails_t_user_1h\":2," +
                         "\"count_risk_declines_user_1h\":3," +
                         "\"count_risk_declines_user_1d\":4" +
                   "}}";
        }

        private static string GetObjWithArrayStub()
        {
            return "{" +
                     "\"phone\": \"+380675163013\"," +
                     "\"card_hash\": \"c486a6fd-597c-4b45-9d37-aaf9837b2e69\"," +
                     "\"documents\": [" +
                                        "{" +
                                          "\"verified\": true," +
                                          "\"type\": \"Passport\"," +
                                          "\"number\": \"EP221241\"" +
                                        "}," +
                                        "{" +
                                          "\"verified\": false," +
                                          "\"type\": \"Passport2\"," +
                                          "\"number\": \"EP221242\"" +
                                        "}" +
                                    "]" +
                   "}";
        }

        private string GetWorkflowDefinition()
        {
            return "<Workflow compressOutput=\"true\">\r\n  <!--START-->\r\n  <Start />\r\n  <Connector name=\"GeoIpConnector_Ip\" connectorName=\"GeoIpConnector\" output=\"GeoIpConnector_Result\" saveAs=\"string\">\r\n    <parameter type=\"appData\" name=\"ip\" value=\"session.ip\" />\r\n  </Connector>\r\n\r\n  <!-- Set ip_country -->\r\n  <DataStore name=\"DefineIpCountry\" output=\"ip_country\" expression=\"'{0}'\">\r\n    <parameter type=\"appData\" name=\"value\" value=\"GeoIpConnector_Result|session.cashier_country\" default=\"0\" />\r\n  </DataStore>\r\n\r\n  <!-- temporary stub until we get the transaction_id (BILL-3886) 20.05.2020 -->\r\n  <DataStore name=\"SetTransactionId\" output=\"transaction_id\" expression=\"'{0}'\">\r\n    <parameter type=\"appData\" name=\"value\" value=\"transaction_id|order_id\" default=\"0\" />\r\n  </DataStore>\r\n\r\n  <Condition name=\"CardMaskIsExist\">\r\n    <test expression=\"'{0}' != '0'\">\r\n      <parameter type=\"appData\" value=\"card_mask\" default=\"0\" />\r\n    </test>\r\n    <iftrue>\r\n      <!--Get bin info-->\r\n      <Connector name=\"BinServiceConnector\" connectorName=\"BinServiceConnector\" output=\"BinInfo\" saveAs=\"jNode\">\r\n        <parameter type=\"appData\" name=\"bin\" value=\"card_mask\" default=\"\" />\r\n      </Connector>\r\n    </iftrue>\r\n  </Condition>\r\n\r\n  <CustomAction name=\"MemoTransactionStoreAction\" output=\"MemoTransactionStoreAction_Result\">\r\n    <parameter type=\"constant\" name=\"transaction_type\" value=\"Withdrawal\" />\r\n    <parameter type=\"appData\" name=\"transaction_id\" value=\"transaction_id\" />\r\n    <parameter type=\"appData\" name=\"order_id\" value=\"order_id\" />\r\n    <parameter type=\"appData\" name=\"billing_country\" value=\"session.cashier_country\" />\r\n    <parameter type=\"appData\" name=\"email\" value=\"pay_method_parameters.user_email|requisites.email.value\" options=\"ToLowerCase\" />\r\n    <parameter type=\"appData\" name=\"phone\" value=\"pay_method_parameters.user_phone|requisites.phone.value\" />\r\n    <parameter type=\"appData\" name=\"device\" value=\"session.agent\" options=\"ToLowerCase\"/>\r\n    <parameter type=\"appData\" name=\"user_id\" value=\"custom_data.player_id\" />\r\n    <parameter type=\"appData\" name=\"session_ip\" value=\"session.ip\" />\r\n    <parameter type=\"appData\" name=\"document_number\" value=\"documents.[0].number\" options=\"ToLowerCase\"/>\r\n    <parameter type=\"appData\" name=\"amount\" value=\"amount\" />\r\n    <parameter type=\"appData\" name=\"device_fingerprint\" value=\"fingerprint\" />\r\n    <parameter type=\"appData\" name=\"user_ip\" value=\"registration.ip\" />\r\n    <parameter type=\"appData\" name=\"currency\" value=\"user.currency\" />\r\n    <parameter type=\"appData\" name=\"ip_country\" value=\"ip_country\" />\r\n    <parameter type=\"appData\" name=\"card_hash\" value=\"pan_token\" />\r\n    <parameter type=\"appData\" name=\"card_country\" value=\"BinInfo.CountryIso\" />\r\n  </CustomAction>\r\n\r\n  <!--DecisionState 1 approved, 2 rejected, 0 indterm-->\r\n  <Condition name=\"IsSophiaActive\">\r\n    <test expression=\"'{0}'\">\r\n      <parameter type=\"appData\" value=\"is_sophia_enabled\" default=\"false\" />\r\n    </test>\r\n    <iftrue>\r\n      <!-- Check is VIP -->\r\n      <DataStore name=\"DefineVIP\" output=\"IsVIP\" expression=\"'{0}' = 'vip-platinum'\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t                            or '{0}' = 'vip-gold'\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t                            or '{0}' = 'vip-silver'\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t                            or '{0}' = 'vip-casino'\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t                            or '{0}' = 'good guys'\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t                            or '{0}' = 'good guys-x'\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t                            or '{0}' = 'vipplatinum'\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t                            or '{0}' = 'vipgold'\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t                            or '{0}' = 'vipsilver'\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t                            or '{0}' = 'vipcasino'\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t                            or '{0}' = 'goodguys'\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t                            or '{0}' = 'goodguysx'\">\r\n        <parameter type=\"appData\" name=\"value\" value=\"user.risk_group\" default=\"unknown\" options=\"ToLowerCase\" />\r\n      </DataStore>\r\n\r\n      <Parallel>\r\n        <!--search by user_id+-->\r\n        <Connector name=\"BlacklistConnector_UserId\" connectorName=\"BlacklistConnector\" output=\"BlacklistConnector_UserId\" saveAs=\"jNode\">\r\n          <parameter type=\"constant\" name=\"key\" value=\"user_id\" />\r\n          <parameter type=\"constant\" name=\"key\" value=\"user.user_id\" />\r\n          <parameter type=\"appData\" name=\"value\" value=\"user.user_id\" />\r\n          <parameter type=\"appData\" name=\"value\" value=\"custom_data.player_id\" />\r\n        </Connector>\r\n\r\n        <!--search by email+-->\r\n        <Connector name=\"BlacklistConnector_Email\" connectorName=\"BlacklistConnector\" output=\"BlacklistConnector_Email\" saveAs=\"jNode\">\r\n          <parameter type=\"appData\" name=\"value\" value=\"requisites.email.value\" />\r\n          <parameter type=\"appData\" name=\"value\" value=\"pay_method_parameters.user_email\" />\r\n          <parameter type=\"appData\" name=\"value\" value=\"pay_method_parameters.email\" />\r\n          <parameter type=\"appData\" name=\"value\" value=\"user.email\" />\r\n        </Connector>\r\n\r\n        <!--search by ip+-->\r\n        <Connector name=\"BlacklistConnector_Ip\" connectorName=\"BlacklistConnector\" output=\"BlacklistConnector_Ip\" saveAs=\"jNode\">\r\n          <parameter type=\"constant\" name=\"key\" value=\"session.ip\" />\r\n          <parameter type=\"constant\" name=\"key\" value=\"registration.ip\" />\r\n          <parameter type=\"appData\" name=\"value\" value=\"pay_method_parameters.send_user_ip\" />\r\n          <parameter type=\"appData\" name=\"value\" value=\"pay_method_parameters.user_ip\" />\r\n          <parameter type=\"appData\" name=\"value\" value=\"registration.ip\" />\r\n          <parameter type=\"appData\" name=\"value\" value=\"session.ip\" />\r\n        </Connector>\r\n\r\n        <!--search by phone-->\r\n        <Connector name=\"BlacklistConnector_Phone\" connectorName=\"BlacklistConnector\" output=\"BlacklistConnector_Phone\" saveAs=\"jNode\">\r\n          <parameter type=\"constant\" name=\"key\" value=\"requisites.phone\" />\r\n          <parameter type=\"constant\" name=\"key\" value=\"user_phone\" />\r\n          <parameter type=\"appData\" name=\"value\" value=\"phone\" />\r\n          <parameter type=\"appData\" name=\"value\" value=\"requisites.phone.value\" />\r\n          <parameter type=\"appData\" name=\"value\" value=\"pay_method_parameters.user_phone\" />\r\n          <parameter type=\"appData\" name=\"value\" value=\"pay_method_parameters.mobile\" />\r\n        </Connector>\r\n\r\n        <!--search by card-->\r\n        <Connector name=\"BlacklistConnector_Card\" connectorName=\"BlacklistConnector\" output=\"BlacklistConnector_Card\" saveAs=\"jNode\">\r\n          <parameter type=\"constant\" name=\"key\" value=\"pan_token\" />\r\n          <parameter type=\"appData\" name=\"value\" value=\"pan_token\" />\r\n        </Connector>\r\n\r\n        <!--search by wallet-->\r\n        <Connector name=\"BlacklistConnector_Wallet\" connectorName=\"BlacklistConnector\" output=\"BlacklistConnector_Wallet\" saveAs=\"jNode\">\r\n          <parameter type=\"appData\" name=\"AllowedWallets\" value=\"$.cashier_parameters[?(@.requisite_type_name == '{0}')].value\" options=\"ListToKeyValue\"/>\r\n        </Connector>\r\n\r\n        <!--WhitelistConnector search by user_id-->\r\n        <Connector name=\"WhitelistConnector_UserId\" connectorName=\"WhitelistConnector\" output=\"WhitelistConnector_UserId\" saveAs=\"jNode\">\r\n          <parameter type=\"constant\" name=\"key\" value=\"user_id\" />\r\n          <parameter type=\"constant\" name=\"key\" value=\"user.user_id\" />\r\n          <parameter type=\"appData\" name=\"value\" value=\"custom_data.player_id\" />\r\n          <parameter type=\"appData\" name=\"value\" value=\"user.user_id\" />\r\n        </Connector>\r\n      </Parallel>\r\n\r\n      <!-- Check UserId is blocked by reason chargeBack -->\r\n      <Condition name=\"DefineUserIdBlockedByChargeBack\">\r\n        <test expression=\"{0} = 2\">\r\n          <parameter type=\"appData\" value=\"BlacklistConnector_Card.[0].origination_id\" default=\"0\" />\r\n        </test>\r\n        <iftrue>\r\n          <DataStore name=\"UserIdBlockedReasonByChargeBack\" output=\"BR_BL_002_ChargeBack\" expression=\"1\" />\r\n        </iftrue>\r\n      </Condition>\r\n      <!-- Check Email is blocked by reason chargeBack -->\r\n      <Condition name=\"DefineEmailBlockedByChargeBack\">\r\n        <test expression=\"{0} = 2\">\r\n          <parameter type=\"appData\" value=\"BlacklistConnector_Email.[0].origination_id\" default=\"0\" />\r\n        </test>\r\n        <iftrue>\r\n          <DataStore name=\"EmailBlockedReasonByChargeBack\" output=\"BR_BL_003_ChargeBack\" expression=\"1\" />\r\n        </iftrue>\r\n      </Condition>\r\n      <!-- Check Phone is blocked by reason chargeBack -->\r\n      <Condition name=\"DefinePhoneBlockedByChargeBack\">\r\n        <test expression=\"{0} = 2\">\r\n          <parameter type=\"appData\" value=\"BlacklistConnector_Phone.[0].origination_id\" default=\"0\" />\r\n        </test>\r\n        <iftrue>\r\n          <DataStore name=\"PhoneBlockedReasonByChargeBack\" output=\"BR_BL_005_ChargeBack\" expression=\"1\" />\r\n        </iftrue>\r\n      </Condition>\r\n\r\n      <Strategy name=\"BlacklistStrategy\" output=\"BlacklistStrategy_Result\" saveAs=\"jNode\" />\r\n\r\n      <!-- Check UserId is Whitelist -->\r\n      <DataStore name=\"DefineUserIdWhitelist\" output=\"UserIdIsWhitelisted\" expression=\"{0} > 0\">\r\n        <parameter type=\"appData\" name=\"value\" value=\"WhitelistConnector_UserId.[0].id\" default=\"0\" />\r\n      </DataStore>\r\n\r\n      <!-- get counters connector-->\r\n      <Connector name=\"MemoWithdrawalCountersConnector\" output=\"MemoWithdrawalCountersConnector_Result\" saveAs=\"jNode\">\r\n        <parameter type=\"appData\" name=\"userId\" value=\"custom_data.player_id\" default=\"0\" />\r\n        <parameter type=\"appData\" name=\"session_ip\" value=\"session.ip\" default=\"0\" />\r\n        <parameter type=\"appData\" name=\"document_number\" value=\"documents.[0].number\" default=\"0\" options=\"ToLowerCase\"/>\r\n        <parameter type=\"appData\" name=\"phone\" value=\"pay_method_parameters.user_phone|requisites.phone.value\" default=\"0\" />\r\n        <parameter type=\"appData\" name=\"email\" value=\"pay_method_parameters.user_email|requisites.email.value\" default=\"0\" options=\"ToLowerCase\" />\r\n        <parameter type=\"appData\" name=\"currency\" value=\"user.currency\" default=\"0\" />\r\n        <parameter type=\"appData\" name=\"card_hash\" value=\"pan_token\" default=\"0\" />\r\n      </Connector>\r\n      <Strategy name=\"WithdrawalConnectorStrategy\" output=\"WithdrawalConnectorStrategy_Result\" saveAs=\"jNode\" />\r\n\r\n      <!-- set score decision -->\r\n      <SelectCase name=\"SetScoreDecision\">\r\n        <Case>\r\n          <test expression=\"{0} >= 80 and {0} &lt; 100\">\r\n            <parameter type=\"appData\" value=\"WithdrawalConnectorStrategy_Result.SCORE\" default=\"0\" />\r\n          </test>\r\n          <iftrue>\r\n            <DataStore name=\"SetScoreAlert\" output=\"RRSCORE02\" expression=\"1\" />\r\n          </iftrue>\r\n        </Case>\r\n        <Case>\r\n          <test expression=\"{0} >= 100\">\r\n            <parameter type=\"appData\" value=\"WithdrawalConnectorStrategy_Result.SCORE\" default=\"0\" />\r\n          </test>\r\n          <iftrue>\r\n            <DataStore name=\"SetScoreReject\" output=\"RRSCORE01\" expression=\"1\" />\r\n          </iftrue>\r\n        </Case>\r\n      </SelectCase>\r\n\r\n      <!-- Blacklist auto-blocking -->\r\n      <Condition name=\"BlacklistAutoBlocking\">\r\n        <test expression=\"('{0}' = 'False' and '{1}' = 'False') and ({2} = 0 and {3} > 0)\">\r\n          <parameter type=\"appData\" value=\"UserIdIsWhitelisted\" default=\"False\" />\r\n          <!-- Check is not UserIdIsWhitelisted -->\r\n          <parameter type=\"appData\" value=\"IsVIP\" default=\"False\" />\r\n          <!-- Check is not VIP -->\r\n          <parameter type=\"appData\" value=\"BlacklistStrategy_Result.BR_BL_001\" default=\"0\" />\r\n          <!-- BlacklistConnector_UserId -->\r\n          <parameter type=\"appData\" value=\"BlacklistStrategy_Result.REJECT_COUNT\" default=\"0\" />\r\n          <!-- Blacklist reject -->\r\n        </test>\r\n        <iftrue>\r\n          <!-- Insert user reject in db -->\r\n          <CustomAction name=\"UserRejectStoreAction\" output=\"UserRejectStoreAction_Result\">\r\n            <parameter type=\"appData\" name=\"user_id\" value=\"custom_data.player_id\" />\r\n            <parameter type=\"appData\" name=\"transaction_id\" value=\"transaction_id|order_id\" default=\"0\"/>\r\n            <parameter type=\"appData\" name=\"decline_reason\" value=\"order_id\" />\r\n          </CustomAction>\r\n\r\n          <!-- Get count user rejecting by last 24h -->\r\n          <Connector name=\"UserRejectConnector\" output=\"UserRejectConnector_Result\" saveAs=\"jNode\">\r\n            <parameter type=\"appData\" name=\"user_id\" value=\"custom_data.player_id\" default=\"0\" />\r\n          </Connector>\r\n\r\n          <!-- Check number of reject by user last 24h -->\r\n          <Condition name=\"CheckNumberOfReject\">\r\n            <test expression=\"{0} > 0\">\r\n              <parameter type=\"appData\" value=\"UserRejectConnector_Result.[2].UserId\" default =\"0\"/>\r\n              <!-- <parameter type=\"strategyData\" value=\"count(UserRejectConnector_Result/*)\" -->\r\n            </test>\r\n            <iftrue>\r\n              <!-- Insert user in blacklist -->\r\n              <CustomAction name=\"BlacklistStoreCustomAction\" output=\"BlacklistStoreCustomAction_Result\">\r\n                <parameter type=\"appData\" name=\"value\" value=\"custom_data.player_id\" />\r\n                <parameter type=\"appData\" name=\"user_id\" value=\"custom_data.player_id\" />\r\n                <parameter type=\"appData\" name=\"transaction_id\" value=\"transaction_id\" />\r\n                <parameter type=\"appData\" name=\"user_rejections\" value=\"UserRejectConnector_Result\" />\r\n\r\n                <parameter type=\"constant\" name=\"key\" value=\"user_id\" />\r\n                <parameter type=\"constant\" name=\"source\" value=\"2\" />\r\n                <parameter type=\"constant\" name=\"expiration\" value=\"\" />\r\n                <parameter type=\"constant\" name=\"project_id\" value=\"\" />\r\n              </CustomAction>\r\n            </iftrue>\r\n          </Condition>\r\n        </iftrue>\r\n      </Condition>\r\n\r\n      <!-- Blacklist decision, 1 rejected -->\r\n      <Condition name=\"BlacklistSetDecision\">\r\n        <test expression=\"{0} > 0 or {1} > 0\">\r\n          <parameter type=\"appData\" value=\"BlacklistStrategy_Result.REJECT_COUNT\" default=\"0\" />\r\n          <parameter type=\"appData\" value=\"BlacklistStoreCustomAction_Result\" default=\"0\" />\r\n        </test>\r\n        <iftrue>\r\n          <DataStore name=\"ReasonMarker\" output=\"RRBS01\" expression=\"1\" />\r\n        </iftrue>\r\n      </Condition>\r\n\r\n      <!--DecisionState 1 approved, 2 rejected, 0 indterm-->\r\n      <Condition name=\"SetDecision\">\r\n        <test expression=\"{0} = 1 and ('{1}' = 'False' and '{2}' = 'False')\">\r\n          <parameter type=\"appData\" value=\"RRBS01\" default=\"0\" />\r\n          <!-- Blacklist reject -->\r\n          <parameter type=\"appData\" value=\"UserIdIsWhitelisted\" default=\"False\" />\r\n          <!-- Check is not IsWhitelisted -->\r\n          <parameter type=\"appData\" value=\"IsVIP\" default=\"False\" />\r\n          <!-- Check is not VIP -->\r\n        </test>\r\n        <iftrue>\r\n          <DataStore name=\"SetRejected\" output=\"DecisionState\" expression=\"2\" />\r\n          <DataStore name=\"ReasonMarker\" output=\"RRBSW01\" expression=\"1\" />\r\n        </iftrue>\r\n        <iffalse>\r\n          <DataStore name=\"SetApproved\" output=\"DecisionState\" expression=\"1\" />\r\n        </iffalse>\r\n      </Condition>\r\n\r\n      <!-- set decision info -->\r\n      <SelectCase name=\"SetDecisionInfo\">\r\n        <Case>\r\n          <test expression=\"{0} > 0 or {1} > 0\">\r\n            <parameter type=\"appData\" value=\"BlacklistStoreCustomAction_Result\" default=\"0\" />\r\n            <parameter type=\"appData\" value=\"BlacklistStrategy_Result.BR_BL_001\" default=\"0\" />\r\n          </test>\r\n          <iftrue>\r\n            <DataStore name=\"SetBlockDecisionInfo\" output=\"decision_info\" expression=\"string('user is blocked')\" />\r\n          </iftrue>\r\n        </Case>\r\n        <Case>\r\n          <test expression=\"{0} = 0 and ({1} = 1 or {2} > 0)\">\r\n            <parameter type=\"appData\" value=\"BlacklistStoreCustomAction_Result\" default=\"0\" />\r\n            <parameter type=\"appData\" value=\"RRSCORE01\" default=\"0\" />\r\n            <!-- Score decision. RRSCORE01:1 = True -->\r\n            <parameter type=\"appData\" value=\"BlacklistStrategy_Result.REJECT_COUNT\" default=\"0\" />\r\n            <!-- Blacklist reject -->\r\n          </test>\r\n          <iftrue>\r\n            <DataStore name=\"SetDeclineDecisionInfo\" output=\"decision_info\" expression=\"string('transaction must be decline')\" />\r\n          </iftrue>\r\n        </Case>\r\n      </SelectCase>\r\n\r\n      <!-- Send message to slack if VIP user frod or used blacklist element. Chanel: sophia-vip-alert -->\r\n      <Condition name=\"SendMessageSlack_VipFrod\">\r\n        <test expression=\"'{0}' = 'True' and ({1} = 1 or {2} > 0)\">\r\n          <parameter type=\"appData\" value=\"IsVIP\" default=\"False\" />\r\n          <!-- Check is VIP -->\r\n          <parameter type=\"appData\" value=\"RRSCORE01\" default=\"0\" />\r\n          <!-- Score decision. RRSCORE01:1 = True -->\r\n          <parameter type=\"appData\" value=\"BlacklistStrategy_Result.REJECT_COUNT\" />\r\n          <!-- Payment attribute blocked -->\r\n        </test>\r\n        <iftrue>\r\n          <!-- Set slack message -->\r\n          <DataStore name=\"SetSlackMessage\" output=\"SlackMessage_VIPfrod\" expression=\"string('*Workflow*: WithdrawalCheck \\n *Subject*: Fraud alert user `{0}` \\n *Body*: Fraud alert transaction `{1}` \\n *Reason*: Blacklist or BR rule')\">\r\n            <parameter type=\"appData\" name=\"value\" value=\"custom_data.player_id\" />\r\n            <parameter type=\"appData\" name=\"value\" value=\"transaction_id|order_id\" default=\"0\"/>\r\n          </DataStore>\r\n          <!--Send message to slack-->\r\n          <CustomAction name=\"SlackAction\" output=\"SendMessageSlack_VipFrod_Result\">\r\n            <parameter type=\"constant\" name=\"slackUrl\" value=\"https://slack.com/api/chat.postMessage\" />\r\n            <parameter type=\"constant\" name=\"channel\" value=\"C035JN6HK71\" />\r\n            <parameter type=\"constant\" name=\"token\" value=\"xoxb-3322160927-1379019340464-qV26kC19k497McaSnC4zI7Gd\" />\r\n            <parameter type=\"constant\" name=\"mentions\" value=\"\" />\r\n            <parameter type=\"appData\" name=\"text\" value=\"SlackMessage_VIPfrod\" />\r\n            <parameter type=\"appData\" name=\"transaction_id\" value=\"transaction_id|order_id\" default=\"0\" />\r\n          </CustomAction>\r\n        </iftrue>\r\n      </Condition>\r\n\r\n      <!-- Send message to slack if was autolock. Chanel: sophia-autolock-alert -->\r\n      <Condition name=\"SendMessageSlack_UserAutolock\">\r\n        <test expression=\"{0} > 0\">\r\n          <parameter type=\"appData\" value=\"BlacklistStoreCustomAction_Result\" default=\"0\" />\r\n          <!-- Autolock -->\r\n        </test>\r\n        <iftrue>\r\n          <!-- Set slack message -->\r\n          <DataStore name=\"SetSlackMessage\" output=\"SlackMessage_Autolock\" expression=\"string('*Workflow*: WithdrawalCheck \\n *Subject*: Autolock user `{0}` \\n *Body*: Last fraud transaction `{1}` \\n *Reason*: Maximum number of fraud transactions has been reached')\">\r\n            <parameter type=\"appData\" name=\"value\" value=\"custom_data.player_id\" />\r\n            <parameter type=\"appData\" name=\"value\" value=\"transaction_id|order_id\" default=\"0\"/>\r\n          </DataStore>\r\n          <!--Send message to slack-->\r\n          <CustomAction name=\"SlackAction\" output=\"SendMessageSlack_UserAutolock_Result\">\r\n            <parameter type=\"constant\" name=\"slackUrl\" value=\"https://slack.com/api/chat.postMessage\" />\r\n            <parameter type=\"constant\" name=\"channel\" value=\"C036U0U0C57\" />\r\n            <parameter type=\"constant\" name=\"token\" value=\"xoxb-3322160927-1379019340464-qV26kC19k497McaSnC4zI7Gd\" />\r\n            <parameter type=\"constant\" name=\"mentions\" value=\"\" />\r\n            <parameter type=\"appData\" name=\"text\" value=\"SlackMessage_Autolock\" />\r\n            <parameter type=\"appData\" name=\"transaction_id\" value=\"transaction_id|order_id\" default=\"0\" />\r\n          </CustomAction>\r\n        </iftrue>\r\n      </Condition>\r\n\r\n      <!-- Send message to slack if whitelist user frod. Chanel: sophia-whitelist-alerts -->\r\n      <Condition name=\"SendMessageSlack_WhitelistFrod\">\r\n        <test expression=\"'{0}' = 'True' and ({1} = 1 or {2} > 0)\">\r\n          <parameter type=\"appData\" value=\"UserIdIsWhitelisted\" default=\"False\" />\r\n          <!-- Check is Whitelisted -->\r\n          <parameter type=\"appData\" value=\"RRSCORE01\" default=\"0\" />\r\n          <!-- Score decision. RRSCORE01:1 = True -->\r\n          <parameter type=\"appData\" value=\"BlacklistStrategy_Result.REJECT_COUNT\" />\r\n          <!-- Counters blacklist attributes -->\r\n        </test>\r\n        <iftrue>\r\n          <!-- Set slack message -->\r\n          <DataStore name=\"SetSlackMessage\" output=\"SlackMessage_WhitelistFrod\" expression=\"string('*Workflow*: WithdrawalCheck \\n *Subject*: Fraud alert user `{0}` \\n *Body*: Fraud alert transaction `{1}` \\n *Reason*: Blacklist or BR rule')\">\r\n            <parameter type=\"appData\" name=\"value\" value=\"custom_data.player_id\" />\r\n            <parameter type=\"appData\" name=\"value\" value=\"transaction_id\" />\r\n          </DataStore>\r\n          <!--Send message to slack-->\r\n          <CustomAction name=\"SlackAction\" output=\"SendMessageSlack_WhitelistFrod_Result\">\r\n            <parameter type=\"constant\" name=\"slackUrl\" value=\"https://slack.com/api/chat.postMessage\" />\r\n            <parameter type=\"constant\" name=\"channel\" value=\"C038U7XSB9B\" />\r\n            <parameter type=\"constant\" name=\"token\" value=\"xoxb-3322160927-1379019340464-qV26kC19k497McaSnC4zI7Gd\" />\r\n            <parameter type=\"constant\" name=\"mentions\" value=\"\" />\r\n            <parameter type=\"appData\" name=\"text\" value=\"SlackMessage_WhitelistFrod\" />\r\n            <parameter type=\"appData\" name=\"transaction_id\" value=\"transaction_id\" />\r\n          </CustomAction>\r\n        </iftrue>\r\n      </Condition>\r\n\r\n      <!--Set REJECT_COUNT flag-->\r\n      <Condition name=\"RejectCountFlag\">\r\n        <test expression=\"{0} > 0\">\r\n          <parameter type=\"appData\" value=\"BlacklistStrategy_Result.REJECT_COUNT\" default=\"0\" />\r\n          <!-- Used blacklist attributes -->\r\n        </test>\r\n        <iftrue>\r\n          <DataStore name=\"RejectFlag\" output=\"RejectCountFlag\" expression=\"1\" />\r\n        </iftrue>\r\n        <iffalse>\r\n          <DataStore name=\"RejectFlag\" output=\"RejectCountFlag\" expression=\"0\" />\r\n        </iffalse>\r\n      </Condition>\r\n\r\n    </iftrue>\r\n    <iffalse>\r\n      <DataStore name=\"SetIndeterminated\" output=\"DecisionState\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n\r\n  <Finish />\r\n</Workflow>";
        }

        private string GetBlacklistStrategyDefinition()
        {
            return "<Strategy>\r\n  <!--START-->\r\n  <!--BR_01 User blacklisted-->\r\n  <Condition>\r\n    <test expression=\"string-length('{0}') > 0\">\r\n      <parameter type=\"appData\" value=\"BlacklistConnector_UserId.[0].id\" />\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_BL_001\" expression=\"1\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_BL_001\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_02 Card blacklisted-->\r\n  <Condition>\r\n    <test expression=\"string-length('{0}') > 0\">\r\n      <parameter type=\"appData\" value=\"BlacklistConnector_Card.[0].id\" />\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_BL_002\" expression=\"1\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_BL_002\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_03 Email blacklisted-->\r\n  <Condition>\r\n    <test expression=\"string-length('{0}') > 0\">\r\n      <parameter type=\"appData\" value=\"BlacklistConnector_Email.[0].id\" />\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_BL_003\" expression=\"1\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_BL_003\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_04 Ip blacklisted-->\r\n  <Condition>\r\n    <test expression=\"string-length('{0}') > 0\">\r\n      <parameter type=\"appData\" value=\"BlacklistConnector_Ip.[0].id\" />\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_BL_004\" expression=\"1\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_BL_004\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_05 Phone blacklisted-->\r\n  <Condition>\r\n    <test expression=\"string-length('{0}') > 0\">\r\n      <parameter type=\"appData\" value=\"BlacklistConnector_Phone.[0].id\" />\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_BL_005\" expression=\"1\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_BL_005\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_06 Fingerprint blacklisted-->\r\n  <Condition>\r\n    <test expression=\"string-length('{0}') > 0\">\r\n      <parameter type=\"appData\" value=\"BlacklistConnector_Fingerprint.[0].id\" />\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_BL_006\" expression=\"1\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_BL_006\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_07 Wallet blacklisted-->\r\n  <Condition>\r\n    <test expression=\"string-length('{0}') > 0\">\r\n      <parameter type=\"appData\" value=\"BlacklistConnector_Wallet.[0].id\" />\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_BL_007\" expression=\"1\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_BL_007\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <Result name=\"REJECT_COUNT\" expression=\"{0}\">\r\n    <parameter type=\"strategyData\" value=\"count(.//*[ starts-with(name(),'BR_BL_') and number(.) > 0 ])\" />\r\n  </Result>\r\n</Strategy>";
        }

        private string GetWithdrawalConnectorStrategyDefinition()
        {
            return "<Strategy>\r\n  <!--START-->\r\n  <Condition>\r\n    <test expression=\"{0} = 0\">\r\n      <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.user_avg_daily_dep_amount_30d\" />\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"user_avg_daily_payout_div_dep_amount_1d\" expression=\"0\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name =\"user_avg_daily_payout_div_dep_amount_1d\" expression=\"{0} div {1}\">\r\n        <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.user_avg_daily_payout_amount_30d\" />\r\n        <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.user_avg_daily_dep_amount_30d\" />\r\n      </Result>\r\n    </iffalse>\r\n  </Condition>\r\n  <Condition>\r\n    <test expression=\"{0} = 0\">\r\n      <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.user_avg_weekly_dep_amount_30d\" />\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"user_avg_weekly_payout_div_dep_amount_30d\" expression=\"0\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name =\"user_avg_weekly_payout_div_dep_amount_30d\" expression=\"{0} div {1}\">\r\n        <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.user_avg_weekly_payout_amount_30d\" />\r\n        <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.user_avg_weekly_dep_amount_30d\" />\r\n      </Result>\r\n    </iffalse>\r\n  </Condition>\r\n  <Condition>\r\n    <test expression=\"{0} = 0\">\r\n      <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.doc_avg_daily_dep_amount_30d\" />\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"doc_avg_daily_payout_div_dep_amount_1d\" expression=\"0\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name =\"doc_avg_daily_payout_div_dep_amount_1d\" expression=\"{0} div {1}\">\r\n        <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.doc_avg_daily_payout_amount_30d\" />\r\n        <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.doc_avg_daily_dep_amount_30d\" />\r\n      </Result>\r\n    </iffalse>\r\n  </Condition>\r\n  <Condition>\r\n    <test expression=\"{0} = 0\">\r\n      <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.doc_avg_weekly_dep_amount_30d\" />\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"doc_avg_weekly_payout_div_dep_amount_30d\" expression=\"0\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name =\"doc_avg_weekly_payout_div_dep_amount_30d\" expression=\"{0} div {1}\">\r\n        <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.doc_avg_weekly_payout_amount_30d\" />\r\n        <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.doc_avg_weekly_dep_amount_30d\" />\r\n      </Result>\r\n    </iffalse>\r\n  </Condition>\r\n  <Condition>\r\n    <test expression=\"'{0}' = 'True'\">\r\n      <parameter type=\"appData\" value=\"IsVIP\" default=\"False\" />\r\n    </test>\r\n    <iftrue>\r\n      <Result name =\"max_payout_amount_1d_limit\" expression=\"{0}\">\r\n        <parameter type=\"settings\" value=\"max_payout_amount_1d_limit_vip\"/>\r\n      </Result>\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name =\"max_payout_amount_1d_limit\" expression=\"{0}\">\r\n        <parameter type=\"settings\" value=\"max_payout_amount_1d_limit_not_vip\"/>\r\n      </Result>\r\n    </iffalse>\r\n  </Condition>\r\n  <Result name =\"min_payout_amount_onetime_limit\" expression=\"{0}\">\r\n    <parameter type=\"settings\" value=\"min_payout_amount_onetime_limit\"/>\r\n  </Result>\r\n  <Result name =\"max_payout_amount_onetime_limit\" expression=\"{0}\">\r\n    <parameter type=\"settings\" value=\"max_payout_amount_onetime_limit\"/>\r\n  </Result>\r\n  <Result name =\"amount_to_euro\" expression=\"{0} div {1}\">\r\n    <parameter type=\"appData\" value=\"amount\" />\r\n    <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.rate_euro\" default=\"1\" />\r\n  </Result>\r\n  <!--BR_WC_001 \"count_ipCountries_card_1d = 2 AND count_ipCountries_card_7d < 3 AND count_ipCountries_card_30d < 3\"-->\r\n  <Condition>\r\n    <test expression=\"{0} = 2 and {1} &lt; 3 and {2} &lt; 3\">\r\n      <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.count_ipCountries_card_1d\" />\r\n      <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.count_ipCountries_card_7d\" />\r\n      <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.count_ipCountries_card_30d\" />\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_WC_SCORE_001\" expression=\"5\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_WC_SCORE_001\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_WC_002 \"count_ipCountries_card_7d > 2 AND count_ipCountries_card_7d < 5 AND count_ipCountries_card_30d < 5\"-->\r\n  <Condition>\r\n    <test expression=\"{0} > 2 and {0} &lt; 5 and {1} &lt; 5\">\r\n      <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.count_ipCountries_card_7d\" />\r\n      <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.count_ipCountries_card_30d\" />\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_WC_SCORE_002\" expression=\"30\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_WC_SCORE_002\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_WC_003 \"count_ipCountries_card_30d > 4\"-->\r\n  <Condition>\r\n    <test expression=\"{0} > 4\">\r\n      <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.count_ipCountries_card_30d\" />\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_WC_SCORE_003\" expression=\"50\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_WC_SCORE_003\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_WC_004 \"count_ipCountries_email_1d = 2 AND count_ipCountries_email_7d < 3 AND count_ipCountries_email_30d < 3\"-->\r\n  <Condition>\r\n    <test expression=\"{0} = 2 and {1} &lt; 3 and {2} &lt; 3\">\r\n      <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.count_ipCountries_email_1d\" />\r\n      <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.count_ipCountries_email_7d\" />\r\n      <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.count_ipCountries_email_30d\" />\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_WC_SCORE_004\" expression=\"5\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_WC_SCORE_004\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_WC_005 \"count_ipCountries_email_7d > 2 and count_ipCountries_email_7d < 5 AND count_ipCountries_email_30d < 5\"-->\r\n  <Condition>\r\n    <test expression=\"{0} > 2 and {0} &lt; 5 and {1} &lt; 5\">\r\n      <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.count_ipCountries_email_7d\" />\r\n      <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.count_ipCountries_email_30d\" />\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_WC_SCORE_005\" expression=\"30\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_WC_SCORE_005\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_WC_006 \"count_ipCountries_email_30d > 4\"-->\r\n  <Condition>\r\n    <test expression=\"{0} > 4\">\r\n      <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.count_ipCountries_email_30d\" />\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_WC_SCORE_006\" expression=\"50\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_WC_SCORE_006\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_WC_007 \"count_ipCountries_phone_1d = 2 AND count_ipCountries_phone_7d < 3 AND count_ipCountries_phone_30d < 3\"-->\r\n  <Condition>\r\n    <test expression=\"{0} = 2 and {1} &lt; 3 and {2} &lt; 3\">\r\n      <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.count_ipCountries_phone_1d\" />\r\n      <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.count_ipCountries_phone_7d\" />\r\n      <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.count_ipCountries_phone_30d\" />\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_WC_SCORE_007\" expression=\"5\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_WC_SCORE_007\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_WC_008 \"count_ipCountries_phone_7d > 2 and count_ipCountries_phone_7d < 5 AND count_ipCountries_phone_30d < 5\"-->\r\n  <Condition>\r\n    <test expression=\"{0} > 2 and {0} &lt; 5 and {1} &lt; 5\">\r\n      <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.count_ipCountries_phone_7d\" />\r\n      <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.count_ipCountries_phone_30d\" />\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_WC_SCORE_008\" expression=\"30\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_WC_SCORE_008\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_WC_009 \"count_ipCountries_phone_30d > 4\"-->\r\n  <Condition>\r\n    <test expression=\"{0} > 4\">\r\n      <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.count_ipCountries_phone_30d\" />\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_WC_SCORE_009\" expression=\"50\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_WC_SCORE_009\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_WC_010 \"count_ipCountries_doc_1d = 2 AND count_ipCountries_doc_7d < 3 AND count_ipCountries_doc_30d < 3\"-->\r\n  <Condition>\r\n    <test expression=\"{0} = 2 and {1} &lt; 3 and {2} &lt; 3\">\r\n      <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.count_ipCountries_doc_1d\" />\r\n      <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.count_ipCountries_doc_7d\" />\r\n      <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.count_ipCountries_doc_30d\" />\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_WC_SCORE_010\" expression=\"5\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_WC_SCORE_010\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_WC_011 \"count_ipCountries_doc_7d > 2 AND count_ipCountries_doc_7d < 5 AND count_ipCountries_doc_30d < 5\"-->\r\n  <Condition>\r\n    <test expression=\"{0} > 2 and {0} &lt; 5 and {1} &lt; 5\">\r\n      <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.count_ipCountries_doc_7d\" />\r\n      <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.count_ipCountries_doc_30d\" />\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_WC_SCORE_011\" expression=\"30\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_WC_SCORE_011\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_WC_012 \"count_ipCountries_doc_30d > 4\"-->\r\n  <Condition>\r\n    <test expression=\"{0} > 4\">\r\n      <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.count_ipCountries_doc_30d\" />\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_WC_SCORE_012\" expression=\"50\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_WC_SCORE_012\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_WC_013 Payout IP country != Payout card BIN country for interval_1d AND Payout card BIN country IS NOT NULL AND Payout IP country IS NOT NULL-->\r\n  <Condition>\r\n    <test expression=\"('{0}' != 'unknown' and '{1}' != 'unknown') and '{0}' != '{1}'\">\r\n      <!--Payout card BIN country-->\r\n      <parameter type=\"appData\" value=\"BinInfo.CountryIso\" default=\"unknown\" />\r\n      <!--ip_country-->\r\n      <parameter type=\"appData\" value=\"ip_country\"  default=\"unknown\" />\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_WC_SCORE_013\" expression=\"30\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_WC_SCORE_013\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_WC_014 payout_amount > max_payout_amount_onetime_limit-->\r\n  <Condition>\r\n    <test expression=\"{0} > {1}\">\r\n      <parameter type=\"strategyData\" value=\"amount_to_euro\"/>\r\n      <parameter type=\"strategyData\" value=\"max_payout_amount_onetime_limit\"/>\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_WC_SCORE_014\" expression=\"10\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_WC_SCORE_014\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_WC_015 payout_amount < min_payout_amount_onetime_limit-->\r\n  <Condition>\r\n    <test expression=\"{0} &lt; {1}\">\r\n      <parameter type=\"strategyData\" value=\"amount_to_euro\"/>\r\n      <parameter type=\"strategyData\" value=\"min_payout_amount_onetime_limit\"/>\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_WC_SCORE_015\" expression=\"100\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_WC_SCORE_015\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_WC_016 payout_amount > max_payout_amount_1d_limit within user_id-->\r\n  <Condition>\r\n    <test expression=\"({0} + {1}) > {2}\">\r\n      <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.sum_payout_amount_to_eur_user_1d\" />\r\n      <parameter type=\"strategyData\" value=\"amount_to_euro\"/>\r\n      <parameter type=\"strategyData\" value=\"max_payout_amount_1d_limit\"/>\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_WC_SCORE_016\" expression=\"100\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_WC_SCORE_016\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_WC_017 payout_amount > max_payout_amount_1d_limit within card_number-->\r\n  <Condition>\r\n    <test expression=\"({0} + {1}) > {2}\">\r\n      <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.sum_payout_amount_to_eur_card_1d\" />\r\n      <parameter type=\"strategyData\" value=\"amount_to_euro\"/>\r\n      <parameter type=\"strategyData\" value=\"max_payout_amount_1d_limit\"/>\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_WC_SCORE_017\" expression=\"100\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_WC_SCORE_017\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_WC_018 payout_amount > max_payout_amount_1d_limit within documentNumber-->\r\n  <Condition>\r\n    <test expression=\"({0} + {1}) > {2}\">\r\n      <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.sum_payout_amount_to_eur_document_1d\" />\r\n      <parameter type=\"strategyData\" value=\"amount_to_euro\"/>\r\n      <parameter type=\"strategyData\" value=\"max_payout_amount_1d_limit\"/>\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_WC_SCORE_018\" expression=\"100\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_WC_SCORE_018\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_WC_019 \"(user_avg_payout_amount_1d / user_avg_dep_amount_1d) >= 3 \r\n             AND (user_avg_payout_amount_1d / user_avg_dep_amount_1d) < 5\"-->\r\n  <Condition>\r\n    <test expression=\"{0} >= 3 and {0} &lt; 5\">\r\n      <parameter type=\"strategyData\" value=\"user_avg_daily_payout_div_dep_amount_1d\"/>\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_WC_SCORE_019\" expression=\"5\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_WC_SCORE_019\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_WC_020 \"(user_avg_payout_amount_1d / user_avg_dep_amount_1d) >= 5 \r\n             AND (user_avg_payout_amount_1d / user_avg_dep_amount_1d) < 7\"-->\r\n  <Condition>\r\n    <test expression=\"{0} >= 5 and {0} &lt; 7\">\r\n      <parameter type=\"strategyData\" value=\"user_avg_daily_payout_div_dep_amount_1d\"/>\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_WC_SCORE_020\" expression=\"30\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_WC_SCORE_020\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_WC_021 (user_avg_payout_amount_1d / user_avg_dep_amount_1d) >= 7-->\r\n  <Condition>\r\n    <test expression=\"{0} >= 7\">\r\n      <parameter type=\"strategyData\" value=\"user_avg_daily_payout_div_dep_amount_1d\"/>\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_WC_SCORE_021\" expression=\"80\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_WC_SCORE_021\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_WC_022 \"(user_avg_payout_amount_7d / user_avg_dep_amount_7d) >= 2 \r\n             AND (user_avg_payout_amount_7d / user_avg_dep_amount_7d) < 3\"-->\r\n  <Condition>\r\n    <test expression=\"{0} >= 2 and {0} &lt; 3\">\r\n      <parameter type=\"strategyData\" value=\"user_avg_weekly_payout_div_dep_amount_30d\"/>\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_WC_SCORE_022\" expression=\"5\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_WC_SCORE_022\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_WC_023 \"(user_avg_payout_amount_7d / user_avg_dep_amount_7d) >= 3 \r\n             AND (user_avg_payout_amount_7d / user_avg_dep_amount_7d) < 4\"-->\r\n  <Condition>\r\n    <test expression=\"{0} >= 3 and {0} &lt; 4\">\r\n      <parameter type=\"strategyData\" value=\"user_avg_weekly_payout_div_dep_amount_30d\"/>\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_WC_SCORE_023\" expression=\"30\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_WC_SCORE_023\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_WC_024 (user_avg_payout_amount_7d / user_avg_dep_amount_7d) >= 4-->\r\n  <Condition>\r\n    <test expression=\"{0} >= 4\">\r\n      <parameter type=\"strategyData\" value=\"user_avg_weekly_payout_div_dep_amount_30d\"/>\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_WC_SCORE_024\" expression=\"50\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_WC_SCORE_024\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_WC_025 \"(doc_avg_payout_amount_1d / doc_avg_dep_amount_1d) >= 3 \r\n             AND (doc_avg_payout_amount_1d / doc_avg_dep_amount_1d) < 5\"-->\r\n  <Condition>\r\n    <test expression=\"{0} >= 3 and {0} &lt; 5\">\r\n      <parameter type=\"strategyData\" value=\"doc_avg_daily_payout_div_dep_amount_1d\"/>\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_WC_SCORE_025\" expression=\"5\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_WC_SCORE_025\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_WC_026 \"(doc_avg_payout_amount_1d / doc_avg_dep_amount_1d) >= 5 \r\n             AND (doc_avg_payout_amount_1d / doc_avg_dep_amount_1d) < 7\"-->\r\n  <Condition>\r\n    <test expression=\"{0} >= 5 and {0} &lt; 7\">\r\n      <parameter type=\"strategyData\" value=\"doc_avg_daily_payout_div_dep_amount_1d\"/>\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_WC_SCORE_026\" expression=\"30\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_WC_SCORE_026\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_WC_027 (doc_avg_payout_amount_1d / doc_avg_dep_amount_1d) >= 7-->\r\n  <Condition>\r\n    <test expression=\"{0} >= 7\">\r\n      <parameter type=\"strategyData\" value=\"doc_avg_daily_payout_div_dep_amount_1d\"/>\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_WC_SCORE_027\" expression=\"50\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_WC_SCORE_027\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_WC_028 \"(doc_avg_payout_amount_7d / doc_avg_dep_amount_7d) >= 2 \r\n             AND (doc_avg_payout_amount_7d / doc_avg_dep_amount_7d) < 3\"-->\r\n  <Condition>\r\n    <test expression=\"{0} >= 2 and {0} &lt; 3\">\r\n      <parameter type=\"strategyData\" value=\"doc_avg_weekly_payout_div_dep_amount_30d\"/>\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_WC_SCORE_028\" expression=\"5\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_WC_SCORE_028\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_WC_029 \"(doc_avg_payout_amount_7d / doc_avg_dep_amount_7d) >= 3 \r\n             AND (doc_avg_payout_amount_7d / doc_avg_dep_amount_7d) < 4\"-->\r\n  <Condition>\r\n    <test expression=\"{0} >= 3 and {0} &lt; 4\">\r\n      <parameter type=\"strategyData\" value=\"doc_avg_weekly_payout_div_dep_amount_30d\"/>\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_WC_SCORE_029\" expression=\"30\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_WC_SCORE_029\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_WC_030 (doc_avg_payout_amount_7d / doc_avg_dep_amount_7d) >= 4-->\r\n  <Condition>\r\n    <test expression=\"{0} >= 4\">\r\n      <parameter type=\"strategyData\" value=\"doc_avg_weekly_payout_div_dep_amount_30d\"/>\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_WC_SCORE_030\" expression=\"50\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_WC_SCORE_030\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_WC_031 \"count_success_payouts_user_1h >= 10 \r\n             AND count_success_payouts_user_1h < 30\"-->\r\n  <Condition>\r\n    <test expression=\"{0} >= 10 and {0} &lt; 30\">\r\n      <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.count_success_payouts_user_1h\" />\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_WC_SCORE_031\" expression=\"50\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_WC_SCORE_031\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_WC_032 \"count_success_payouts_user_1d >= 30 \r\n             AND count_success_payouts_user_1d < 50\"-->\r\n  <Condition>\r\n    <test expression=\"{0} >= 30 and {0} &lt; 50\">\r\n      <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.count_success_payouts_user_1d\" />\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_WC_SCORE_032\" expression=\"80\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_WC_SCORE_032\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_WC_033 count_success_payouts_user_7d >= 50-->\r\n  <Condition>\r\n    <test expression=\"{0} >= 50\">\r\n      <parameter type=\"appData\" value=\"MemoWithdrawalCountersConnector_Result.count_success_payouts_user_7d\" />\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_WC_SCORE_033\" expression=\"30\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_WC_SCORE_033\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n  <!--BR_WC_034 \"by country iso\"-->\r\n  <Condition>\r\n    <test expression=\"'{0}' = 'ru' or '{0}' = 'rus'\">\r\n      <parameter type=\"appData\" value=\"BinInfo.CountryIso\" options=\"ToLowerCase\"/>\r\n    </test>\r\n    <iftrue>\r\n      <Result name=\"BR_WC_SCORE_034\" expression=\"100\" />\r\n    </iftrue>\r\n    <iffalse>\r\n      <Result name=\"BR_WC_SCORE_033\" expression=\"0\" />\r\n    </iffalse>\r\n  </Condition>\r\n\r\n  <!-- sum score -->\r\n  <Result name=\"SCORE\" expression=\"{0}\">\r\n    <parameter type=\"strategyData\" value=\"sum(.//*[starts-with(name(),'BR_WC_SCORE_')])\" />\r\n  </Result>\r\n</Strategy>";
        }
        #endregion
    }
}
