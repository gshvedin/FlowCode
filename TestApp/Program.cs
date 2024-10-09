using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using WorkflowEngine;
using WorkflowEngine.Core.Dependencies.Connectors;
using WorkflowEngine.Core.Dependencies.Counters;
using WorkflowEngine.Core.Dependencies.CustomActions;
using WorkflowEngine.Core.Dependencies.CustomFunctions;
using WorkflowEngine.Core.Dependencies.Lists;
using WorkflowEngine.Core.Dependencies.Strategies;
using WorkflowEngine.Core.Dependencies.WorkflowProcedures;
using WorkflowEngine.Helpers.Audit;


namespace TestApp
{

    public class Program
    {
        static void Main(string[] args)
        {
            Execute();
        }



        private static void Execute()
        {
            Console.WriteLine("Save workflow output to data.json file? (y/n)");
            var p = Console.ReadKey().KeyChar.ToString().ToLower();
            bool saveContext = p == "y";
            //retrieve data json
            string data = System.IO.File.ReadAllText("TestData/data.json");
            //retrieve workflow descriptor xml
            string workflow = System.IO.File.ReadAllText("TestData/wf.xml");

            //execute workflow
            var dc = GetDependencyContainer();
            var wf = new Workflow(workflow, dc);
            //wf.CurrentInstance.SaveUserTaskTracking = true;
            var res = wf
                .ExecuteAsync(data, CancellationToken.None)
                .Result;
            if (saveContext)
                System.IO.File.WriteAllText("TestData/data.json", res);
            var compressed = wf.CurrentInstance.ContextData.Data.ToString();
            Console.WriteLine(compressed);

            Console.WriteLine("Show audit items? (y/n)");
            p = Console.ReadKey().KeyChar.ToString().ToLower();
            if (p == "y")
                Console.WriteLine(GetAudit(wf.CurrentInstance.GetAuditItems()));
            else
                Console.WriteLine("Press any key to exit");
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
                       .AddSingleton<IWorkflowProcedureService, WorkflowProcedureService>()
                       .AddScoped<ICustomFunctionProvider, CustomFunctionsProvider>()
                       .BuildServiceProvider();

            return new WorkflowDependecyContainer(serviceProvider)
            {
                MetaInfo = new Dictionary<string, object>() { { "RequestId", Guid.NewGuid() } },
            };
        }


        static string GetAudit(IList<WorkflowAuditItem> items)
        {
            // Example list of audit items
            var auditItems = items.OrderBy(ai => ai.AuditId).ToList();

            // Serializing the tree to JSON
            var json = JsonConvert.SerializeObject(auditItems.Select(ai => new
            {
                AuditId = ai.AuditId,
                ActionType = ai.ActionType,
                Name = ai.Name,
                Timestamp = ai.Timestamp,
                Info = ai.Info,
                State = ai.State.ToString(),
                Depth = ai.Depth
            }), Newtonsoft.Json.Formatting.Indented);
            return json;

        }
    }





}
