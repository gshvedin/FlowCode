using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using WorkflowEngine.Helpers.Audit;
using WorkflowEngine.Core.Dependencies;
using WorkflowEngine.Core.Dependencies.Connectors;
using WorkflowEngine.Core.Dependencies.Strategies;
using WorkflowEngine.Core.Evaluation;
using WorkflowEngine.Core.Dependencies.CustomActions;
using System.Threading.Tasks;
using System.Threading;

namespace WorkflowEngine.Test.ActionIntegrationTests
{
#pragma warning disable SA1402 // File may only contain a single type
    public class DC : IDependencyContainer
#pragma warning restore SA1402 // File may only contain a single type
    {
        private static DC dC;

        private static object syncRoot = new object();

        public Dictionary<string, object> MetaInfo { get; set; }

        public IServiceProvider ServiceProvider { get; set; }

        public static DC GetInstance()
        {
            if (dC == null)
            {
                lock (syncRoot)
                {
                    if (dC == null)
                    {
                        dC = InitialDependencyContainer();
                    }
                }
            }

            return dC;
        }

        public static DC InitialDependencyContainerReturnNonSupportiveStrategy()
        {
            ServiceProvider serviceProvider = new ServiceCollection()
                                 .AddSingleton<IConnectorFactory, ConnectorFactory>()
                                 .AddSingleton<IWorkflowAuditService, WorkFlowAudit>()
                                 .AddSingleton<IStrategyService, StrategyServiceReturnNonSupportiveStrategy>()
                                 .AddSingleton<ICustomActionFactory, CustomActionFactory>()
                                 .AddSingleton<ICustomAction, CustomAction>()
                                 .BuildServiceProvider();
            return new DC()
            {
                ServiceProvider = serviceProvider,
                MetaInfo = new Dictionary<string, object>() { { "RequestId", Guid.NewGuid() } }
            };
        }

        private static DC InitialDependencyContainer()
        {
            ServiceProvider serviceProvider = new ServiceCollection()
                                 .AddSingleton<IConnectorFactory, ConnectorFactory>()
                                 .AddSingleton<IWorkflowAuditService, WorkFlowAudit>()
                                 .AddSingleton<IStrategyService, StrategyService>()
                                 .AddSingleton<ICustomActionFactory, CustomActionFactory>()
                                 .AddSingleton<ICustomAction, CustomAction>()
                                 .BuildServiceProvider();
            return new DC()
            {
                ServiceProvider = serviceProvider,
                MetaInfo = new Dictionary<string, object>() { { "RequestId", Guid.NewGuid() } }
            };
        }
    }

#pragma warning disable SA1402 // File may only contain a single type
    public class BlackListConnector : IConnector
#pragma warning restore SA1402 // File may only contain a single type
    {
        public async Task<string> ExecuteAsync(Parameters parameters, CancellationToken cancellationToken = default)
        {
            string key = parameters.GetParameter<string>("key");
            string value = parameters.GetParameter<string>("value");
            int groupId = parameters.GetParameterOrDefault("groupId", -1);

            // call search API controller
            string res = "{" +
                            $"'key': {key}, " +
                            $"'value': {value}, " +
                            $"'groupId': {groupId}" +
                         "}";

            return await Task.FromResult(res);
        }
    }

#pragma warning disable SA1402 // File may only contain a single type
    public class ConnectorFactory : IConnectorFactory
#pragma warning restore SA1402 // File may only contain a single type
    {
        public IConnector Resolve(string name)
        {
            return new BlackListConnector();
        }
    }

#pragma warning disable SA1402 // File may only contain a single type
    public class WorkFlowAudit : IWorkflowAuditService
#pragma warning restore SA1402 // File may only contain a single type
    {
        public async Task AddAuditItem(WorkflowAuditItem item)
        {
            await Task.Delay(1);

            Console.WriteLine(item.ToString());
        }

        public async Task AddAuditItems(IList<WorkflowAuditItem> auditItems, Dictionary<string, object> metaInfo)
        {
            await Task.Delay(1);

            foreach (WorkflowAuditItem item in auditItems)
            {
                Console.WriteLine(item.ToString());
            }

            foreach (KeyValuePair<string, object> item in metaInfo)
            {
                Console.WriteLine($"key: {item.Key} - value: {item.Value}");
            }
        }

        public async Task<IEnumerable<WorkflowAuditItem>> GetAudit(Guid requestId)
        {
            Console.WriteLine(requestId);

            return await Task.FromResult(default(IEnumerable<WorkflowAuditItem>));
        }
    }

#pragma warning disable SA1402 // File may only contain a single type
    public class StrategyService : IStrategyService
#pragma warning restore SA1402 // File may only contain a single type
    {
        public async Task<string> GetStrategyDefinitionAsync(string name)
        {
            string wfData = System.IO.File.ReadAllText("Files/Strategy.xml");
            return await Task.FromResult(wfData);
        }
    }

#pragma warning disable SA1402 // File may only contain a single type
    public class StrategyServiceReturnNonSupportiveStrategy : IStrategyService
#pragma warning restore SA1402 // File may only contain a single type
    {
        public async Task<string> GetStrategyDefinitionAsync(string name)
        {
            string wfData = System.IO.File.ReadAllText("Files/StrategyThrowException.xml");
            return await Task.FromResult(wfData);
        }
    }

#pragma warning disable SA1402 // File may only contain a single type
    public class CustomActionFactory : ICustomActionFactory
#pragma warning restore SA1402 // File may only contain a single type
    {
        public ICustomAction Resolve(string name)
        {
            return new CustomAction();
        }
    }

#pragma warning disable SA1402 // File may only contain a single type
    public class CustomAction : ICustomAction
#pragma warning restore SA1402 // File may only contain a single type
    {
        public async Task<string> ExecuteAsync(Parameters parameters, CancellationToken cancellationToken = default)
        {
            string res = "{" +
                            $"\"count\": {parameters.Count}, " +
                            $"\"parameter1Name\": {parameters[0].Name}, " +
                            $"\"parameter2Name\": {parameters[1].Name}" +
                         "}";

            return await Task.FromResult(res);
        }
    }
}
