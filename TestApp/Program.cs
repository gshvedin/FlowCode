using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using WorkflowEngine;
using WorkflowEngine.Core.Dependencies.Connectors;
using WorkflowEngine.Core.Dependencies.Counters;
using WorkflowEngine.Core.Dependencies.CustomActions;
using WorkflowEngine.Core.Dependencies.Lists;
using WorkflowEngine.Core.Dependencies.Strategies;
using WorkflowEngine.Helpers.Audit;

namespace TestApp
{

    public class Program
    {

        //states create processes, processes create states
        public static void Main(string[] args)
        {
            Execute();
        }

        private static void Execute()
        {
            //retrieve data json
            string data = System.IO.File.ReadAllText("data.json");
            //retrieve workflow descriptor xml
            string workflow = System.IO.File.ReadAllText("wf.xml");

            //execute workflow
            var dc = GetDependencyContainer();
            var wf = new Workflow(workflow, dc);
            var res = wf
                .ExecuteAsync(data, CancellationToken.None)
                .Result;
            var compressed = wf.CurrentInstance.ContextData.CompressedData.ToString();
            bool co = wf.CurrentInstance.CompressOutput;
            Console.ReadLine();

        }

        private static WorkflowDependecyContainer GetDependencyContainer()
        {
            ServiceProvider serviceProvider = new ServiceCollection()
                       .AddSingleton<IConnectorFactory, ConnectorFactory>()
                       .AddSingleton<ICustomActionFactory, CustomActionFactory>()
                       .AddSingleton<IStrategyService, StrategyService>()
                       .AddSingleton<IWorkflowAuditService, WorkFlowAudit>()
                       .AddSingleton<IListService, ListService>()
                       .AddSingleton<ICounterService, CounterService>()
                       .BuildServiceProvider();

            return new WorkflowDependecyContainer()
            {
                MetaInfo = new Dictionary<string, object>() { { "RequestId", Guid.NewGuid() } },
                ServiceProvider = serviceProvider
            };
        }


    }





}
