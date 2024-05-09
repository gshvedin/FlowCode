using Microsoft.Extensions.DependencyInjection;
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

namespace TestApp
{

    public class Program
    {

        //states create processes, processes create states
        public static void Main(string[] args)
        {
            ReadDir();
            Console.WriteLine("ss");
        }

        private static void ReadDir()
        {
            Console.WriteLine("Введите путь к директории:");
            string dirPath = Console.ReadLine();

            if (Directory.Exists(dirPath))
            {
                Console.WriteLine("Введите имя текстового файла для сохранения списка (например, output.txt):");
                string outputFile = Console.ReadLine();

                try
                {
                    using (StreamWriter writer = new StreamWriter(outputFile))
                    {
                        WriteDirContents(dirPath, writer, "");
                    }

                    Console.WriteLine($"Список файлов и папок был сохранен в {outputFile}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Произошла ошибка: {e.Message}");
                }
            }
            else
            {
                Console.WriteLine("Указанная директория не существует.");
            }
        }

        
 
static void WriteDirContents(string dirPath, StreamWriter writer, string indent)
        {
            string[] files = Directory.GetFiles(dirPath);
            string[] directories = Directory.GetDirectories(dirPath);

            foreach (string file in files)
            {
                writer.WriteLine(indent + Path.GetFileName(file));
            }

            foreach (string directory in directories)
            {
                writer.WriteLine(indent + Path.GetFileName(directory) + "/");
                WriteDirContents(directory, writer, indent + "  ");
            }
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

            return new WorkflowDependecyContainer(serviceProvider)
            {
                MetaInfo = new Dictionary<string, object>() { { "RequestId", Guid.NewGuid() } },
            };
        }


    }





}
