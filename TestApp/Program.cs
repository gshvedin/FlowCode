using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using WorkflowEngine;
using WorkflowEngine.Core.Dependencies.Connectors;
using WorkflowEngine.Core.Dependencies.Counters;
using WorkflowEngine.Core.Dependencies.CustomActions;
using WorkflowEngine.Core.Dependencies.Lists;
using WorkflowEngine.Core.Dependencies.Strategies;
using WorkflowEngine.Helpers.Audit;
using WorkflowEngine.Helpers;
using WorkflowEngine.Core.Dependencies.WorkflowProcedures;

namespace TestApp
{

    public class Program
    {

        //states create processes, processes create states
        public static void Main(string[] args)
        {
            //retrieve data json
            //string data = System.IO.File.ReadAllText("TestData/data.json");
            //JObject jobg = JObject.Parse(data);
            //jobg.AddOrReplaceByPath("$.token.layeruno.1.value", "some valeu");
            //Console.WriteLine(jobg.ToString());
            //jobg.AddOrReplaceByPath("$..token.layeruno", "some valeu2");

            Execute();
            Console.WriteLine("Enter any key to quit");
            Console.ReadKey();
        }

     
        private static void Execute()
        {
            //retrieve data json
            string data = System.IO.File.ReadAllText("TestData/data.json");
            //retrieve workflow descriptor xml
            string workflow = System.IO.File.ReadAllText("TestData/wf.xml");

            //execute workflow
            var dc = GetDependencyContainer();
            var wf = new Workflow(workflow, dc);
            var res = wf
                .ExecuteAsync(data, CancellationToken.None)
                .Result;
            var compressed = wf.CurrentInstance.ContextData.CompressedData.ToString();
            Console.WriteLine(compressed);

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
                       .AddSingleton<IWorkflowProcedureService, WorkflowProcedureService>()
                       .BuildServiceProvider();

            return new WorkflowDependecyContainer(serviceProvider)
            {
                MetaInfo = new Dictionary<string, object>() { { "RequestId", Guid.NewGuid() } },
            };
        }


    }





}
