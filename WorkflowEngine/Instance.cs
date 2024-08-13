using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using WorkflowEngine.Core;
using WorkflowEngine.Core.Dependencies;
using WorkflowEngine.Core.Dependencies.Connectors;
using WorkflowEngine.Core.Dependencies.Counters;
using WorkflowEngine.Core.Dependencies.CustomActions;
using WorkflowEngine.Core.Dependencies.Lists;
using WorkflowEngine.Core.Dependencies.Strategies;
using WorkflowEngine.Core.WorkflowProcedures;
using WorkflowEngine.Helpers.Audit;

namespace WorkflowEngine
{
    public class Instance : IInstance
    {
        public CancellationToken CancellationToken { get; set; }

        public int MaxDegreeOfParallelism { get; set; } = 10;

        public int CountersDefaultTtl { get; set; } = 43200;

        public bool SaveUserTaskTracking { get; set; } = true;

        public IDictionary<string, List<CounterData>> CountersCache { get; set; } = new Dictionary<string, List<CounterData>>();

        public IContextData ContextData { get; set; }

        public IDependencyContainer DC { get; set; }

        private IList<WorkflowAuditItem> AuditItems { get; set; } = new List<WorkflowAuditItem>();

        public IWorkflowProceduresList WorkflowProcedures { get; set; }

        public bool CompressOutput
        {
            get
            {
                XDocument wfXml = XDocument.Parse(WorkflowDefinition);
                if (wfXml.Root.Attribute("compressOutput")?.Value is string value && bool.TryParse(value, out bool result))
                {
                    return result;
                }

                return false;
            }
        }

        public string WorkflowDefinition { get; set; }

        public void SetDependencies(IDependencyContainer dc)
        {
            DC = dc;
        }

        public T GetDependency<T>() where T : class
        {
            return DC?.Resolve<T>();
        }

        public bool CheckDependencies()
        {
            bool checkResult = true;

            if (DC == null)
            {
                System.Console.WriteLine("Dependencies container is null");
                checkResult = false;
            }

            if (GetDependency<IStrategyService>() == null)
            {
                System.Console.WriteLine("Strategy Service not registered");
                checkResult = false;
            }

            if (GetDependency<IConnectorFactory>() == null)
            {
                System.Console.WriteLine("Connector Factory is null");
                checkResult = false;
            }

            if (GetDependency<ICustomActionFactory>() == null)
            {
                System.Console.WriteLine("CustomAction Factory is null");
                checkResult = false;
            }

            if (GetDependency<IListService>() == null)
            {
                System.Console.WriteLine("ListService is null");
                checkResult = false;
            }

            if (GetDependency<IWorkflowAuditService>() == null)
            {
                System.Console.WriteLine("WorkFlowAudit Service is null");
                checkResult = false;
            }

            return checkResult;
        }

        public void AddAuditItem(WorkflowAuditItem item)
        {
            if (AuditItems.Count == 0)
            {
                item.AuditId = 1;
            }
            else
            {
                item.AuditId = AuditItems.Max(t => t.AuditId) + 1;
            }
            AuditItems.Add(item);
        }

        public IList<WorkflowAuditItem> GetAuditItems()
        {
            return AuditItems;
        }
    }
}
