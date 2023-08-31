using AutoFixture.Xunit2;
using Moq;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using WorkflowEngine.Actions.Implementations;
using WorkflowEngine.Core;
using WorkflowEngine.Core.Dependencies;
using WorkflowEngine.Core.Dependencies.Connectors;
using WorkflowEngine.Core.Dependencies.Counters;
using WorkflowEngine.Core.Dependencies.CustomActions;
using WorkflowEngine.Core.Dependencies.Lists;
using WorkflowEngine.Core.Dependencies.Strategies;
using WorkflowEngine.Core.WorkflowProcedures;
using WorkflowEngine.Helpers.Audit;
using Xunit;

namespace WorkflowEngine.Test
{
    public class InstanceTest
    {
        [Theory, AutoData]
        public void MaxDegreeOfParallelism_SetValue_Successfully(int maxDegreeOfParallelism)
        {
            // Arrange
            Instance instance = new Instance
            {
                // Act
                MaxDegreeOfParallelism = maxDegreeOfParallelism,
            };

            // Assert
            Assert.Equal(maxDegreeOfParallelism, instance.MaxDegreeOfParallelism);
        }

        [Fact]
        public void MaxDegreeOfParallelism_GetDefaultValue_Return10()
        {
            // Arrange
            Instance instance = new Instance();

            // Act
            // Assert
            Assert.Equal(10, instance.MaxDegreeOfParallelism);
        }

        [Theory, AutoData]
        public void ContextData_SetValue_Successfully(Mock<IContextData> mockContextData)
        {
            // Arrange
            Instance instance = new Instance
            {
                // Act
                ContextData = mockContextData.Object
            };

            // Assert
            Assert.Equal(mockContextData.Object, instance.ContextData);
        }

        [Theory, AutoData]
        public void SetDependencies_SetValue_Successfully(Mock<IDependencyContainer> mockDependencyContainer)
        {
            // Arrange
            Instance instance = new Instance();

            // Act
            instance.SetDependencies(mockDependencyContainer.Object);

            // Assert
            Assert.Equal(mockDependencyContainer.Object, instance.DC);
        }

        [Theory, AutoData]
        public void GetDependency_SetValue_Successfully(
                                        Mock<IServiceProvider> mockServiceProvider,
                                        Mock<IDependencyContainer> mockDependencyContainer,
                                        XElement xElement)
        {
            // Arrange
            ConnectorAction connectorAction = new ConnectorAction(xElement);
            mockServiceProvider.Setup(m => m.GetService(It.IsAny<Type>()))
                               .Returns(connectorAction);
            mockDependencyContainer.SetupGet(m => m.ServiceProvider)
                                   .Returns(mockServiceProvider.Object);
            Instance instance = new Instance();
            instance.SetDependencies(mockDependencyContainer.Object);

            // Act
            ConnectorAction result = instance.GetDependency<ConnectorAction>();

            // Assert
            Assert.Equal(connectorAction, result);
        }

        [Theory, AutoData]
        public void GetDependency_SetValueNull_ReturnNull()
        {
            // Arrange
            Instance instance = new Instance();
            instance.SetDependencies(null);

            // Act
            ConnectorAction result = instance.GetDependency<ConnectorAction>();

            // Assert
            Assert.Null(result);
        }

        [Theory, AutoData]
        public void CountersCache_SetValue_Successfully(IDictionary<string, List<CounterData>> countersCache)
        {
            // Arrange
            Instance instance = new Instance
            {
                // Act
                CountersCache = countersCache,
            };

            // Assert
            Assert.Equal(countersCache, instance.CountersCache);
        }

        [Fact]
        public void CountersCache_GetDefaultValue_ReturnCorrect()
        {
            // Arrange
            Instance instance = new Instance();

            // Act
            // Assert
            Assert.Empty(instance.CountersCache);
        }

        [Theory, AutoData]
        public void CountersDefaultTtl_SetValue_Successfully(int countersDefaultTtl)
        {
            // Arrange
            Instance instance = new Instance
            {
                // Act
                CountersDefaultTtl = countersDefaultTtl,
            };

            // Assert
            Assert.Equal(countersDefaultTtl, instance.CountersDefaultTtl);
        }

        [Fact]
        public void CountersDefaultTtl_GetDefaultValue_Return43200()
        {
            // Arrange
            Instance instance = new Instance();

            // Act
            // Assert
            Assert.Equal(43200, instance.CountersDefaultTtl);
        }

        [Theory, AutoData]
        public void AuditItems_SetValue_Successfully(IList<WorkflowAuditItem> workflowAuditItems)
        {
            // Arrange
            Instance instance = new Instance
            {
                // Act
                AuditItems = workflowAuditItems,
            };

            // Assert
            Assert.Equal(workflowAuditItems, instance.AuditItems);
        }

        [Fact]
        public void WorkflowProcedures_SetValue_ReturnCorrect()
        {
            // Arrange
            Mock<IWorkflowProceduresList> procedureList = new Mock<IWorkflowProceduresList>();

            Instance instance = new Instance
            {
                // Act
                WorkflowProcedures = procedureList.Object,
            };

            // Assert
            Assert.Equal(procedureList.Object, instance.WorkflowProcedures);
        }

        [Fact]
        public void CompressOutput_GetValue_ReturnTrue()
        {
            // Arrange
            Instance instance = new Instance();

            // Act
            Workflow wfCompressed = new Workflow("<Workflow compressOutput=\"true\"><Start/><Finish/></Workflow>", instance);

            // Act
            // Assert
            Assert.Equal(true, instance.CompressOutput);
        }

        [Fact]
        public void CompressOutput_GetValue_ReturnFalse()
        {
            // Arrange
            Instance instance = new Instance();

            // Act
            Workflow wfCompressed = new Workflow("<Workflow><Start/><Finish/></Workflow>", instance);

            // Act
            // Assert
            Assert.Equal(false, instance.CompressOutput);
        }

        [Fact]
        public void CheckDependencies_GetValue_DcNotExists_ReturnFalse()
        {
            // Arrange
            Instance instance = new Instance()
            {
                DC = GetDependencyContainer(dcExists: false)
            };

            // Act
            bool result = instance.CheckDependencies();

            // Assert
            Assert.Equal(false, result);
        }

        [Fact]
        public void CheckDependencies_GetValue_StrategyServiceNotExists_ReturnFalse()
        {
            // Arrange
            Instance instance = new Instance()
            {
                DC = GetDependencyContainer(dcExists: true, strategyServiceExists: false)
            };

            // Act
            bool result = instance.CheckDependencies();

            // Assert
            Assert.Equal(false, result);
        }

        [Fact]
        public void CheckDependencies_GetValue_ConnectorFactoryNotExists_ReturnFalse()
        {
            // Arrange
            Instance instance = new Instance()
            {
                DC = GetDependencyContainer(
                    dcExists: true,
                    strategyServiceExists: true,
                    connectorFactoryExists: false)
            };

            // Act
            bool result = instance.CheckDependencies();

            // Assert
            Assert.Equal(false, result);
        }

        [Fact]
        public void CheckDependencies_GetValue_CustomActionFactoryNotExists_ReturnFalse()
        {
            // Arrange
            Instance instance = new Instance()
            {
                DC = GetDependencyContainer(dcExists: true, strategyServiceExists: true, connectorFactoryExists: true, customActionFactoryExists: false)
            };

            // Act
            bool result = instance.CheckDependencies();

            // Assert
            Assert.Equal(false, result);
        }

        [Fact]
        public void CheckDependencies_GetValue_ListServiceNotExists_ReturnFalse()
        {
            // Arrange
            Instance instance = new Instance()
            {
                DC = GetDependencyContainer(
                    dcExists: true,
                    strategyServiceExists: true,
                    connectorFactoryExists: true,
                    customActionFactoryExists: true,
                    listServiceExists: false)
            };

            // Act
            bool result = instance.CheckDependencies();

            // Assert
            Assert.Equal(false, result);
        }

        [Fact]
        public void CheckDependencies_GetValue_WorkflowAuditServiceExistsNotExists_ReturnFalse()
        {
            // Arrange
            Instance instance = new Instance()
            {
                DC = GetDependencyContainer(
                    dcExists: true,
                    strategyServiceExists: true,
                    connectorFactoryExists: true,
                    customActionFactoryExists: true,
                    listServiceExists: true,
                    workflowAuditServiceExists: false)
            };

            // Act
            bool result = instance.CheckDependencies();

            // Assert
            Assert.Equal(false, result);
        }

        [Fact]
        public void CheckDependencies_GetValue_AlLServicesExists_ReturnTrue()
        {
            // Arrange
            Instance instance = new Instance()
            {
                DC = GetDependencyContainer(
                    dcExists: true,
                    strategyServiceExists: true,
                    connectorFactoryExists: true,
                    customActionFactoryExists: true,
                    listServiceExists: true,
                    workflowAuditServiceExists: true)
            };

            // Act
            bool result = instance.CheckDependencies();

            // Assert
            Assert.Equal(true, result);
        }

        [Fact]
        public void GetDependency_GetValue_IStrategyService_ReturnCorrect()
        {
            // Arrange
            Instance instance = new Instance()
            {
                DC = GetDependencyContainer(
                    dcExists: true,
                    strategyServiceExists: true)
            };

            // Act
            IStrategyService result = instance.GetDependency<IStrategyService>();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void SetMetaInfo_DcNotExists_ReturnFalse()
        {
            // Arrange
            Instance instance = new Instance()
            {
                DC = GetDependencyContainer(
                    dcExists: false)
            };

            Workflow wf = new Workflow("<Workflow></Workflow>", instance);

            // Act
            bool result = wf.SetMetaInfo(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void SetMetaInfo_MetaInfoIsNull_ReturnFalse()
        {
            // Arrange
            Instance instance = new Instance()
            {
                DC = GetDependencyContainer(
                    dcExists: true)
            };

            Workflow wf = new Workflow("<Workflow></Workflow>", instance);

            // Act
            bool result = wf.SetMetaInfo(null);

            // Assert
            Assert.False(result);
        }

        [Theory, AutoData]
        public void SetMetaInfo_AllDataCorrect_ReturnTrue(Dictionary<string, object> metainfo)
        {
            // Arrange
            Instance instance = new Instance()
            {
                DC = GetDependencyContainer(
                    dcExists: true)
            };

            Workflow wf = new Workflow("<Workflow></Workflow>", instance);

            // Act
            bool result = wf.SetMetaInfo(metainfo);

            // Assert
            Assert.True(result);
        }

        private IDependencyContainer GetDependencyContainer(
            bool dcExists = false,
            bool strategyServiceExists = false,
            bool connectorFactoryExists = false,
            bool customActionFactoryExists = false,
            bool listServiceExists = false,
            bool workflowAuditServiceExists = false)
        {
            if (!dcExists)
            {
                return null;
            }

            Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();
            serviceProvider = RegisterService<IStrategyService>(serviceProvider, strategyServiceExists);
            serviceProvider = RegisterService<IConnectorFactory>(serviceProvider, connectorFactoryExists);
            serviceProvider = RegisterService<ICustomActionFactory>(serviceProvider, customActionFactoryExists);
            serviceProvider = RegisterService<IListService>(serviceProvider, listServiceExists);
            serviceProvider = RegisterService<IWorkflowAuditService>(serviceProvider, workflowAuditServiceExists);

            Mock<IDependencyContainer> dependencyContainer = new Mock<IDependencyContainer>();
            dependencyContainer.Setup(x => x.ServiceProvider)
               .Returns(serviceProvider.Object);

            return dependencyContainer.Object;
        }

        private Mock<IServiceProvider> RegisterService<T>(Mock<IServiceProvider> serviceProvider, bool exists = false) where T : class
        {
            T mockedObject = new Mock<T>().Object;
            serviceProvider.Setup(x => x.GetService(typeof(T))).Returns(exists ? mockedObject : null);
            return serviceProvider;
        }
    }
}
