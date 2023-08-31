using System.Collections.Generic;
using System.Threading;
using WorkflowEngine.Core;
using WorkflowEngine.Core.Dependencies;
using WorkflowEngine.Core.Dependencies.Counters;
using WorkflowEngine.Core.WorkflowProcedures;
using WorkflowEngine.Helpers.Audit;

namespace WorkflowEngine
{
    public interface IInstance
    {
        CancellationToken CancellationToken { get; set; }

        bool CompressOutput { get; }

        string WorkflowDefinition { get; set; }

        IContextData ContextData { get; set; }

        int MaxDegreeOfParallelism { get; set; }

        int CountersDefaultTtl { get; set; }

        IDictionary<string, List<CounterData>> CountersCache { get; set; }

        public IDependencyContainer DC { get; set; }

        IList<WorkflowAuditItem> AuditItems { get; set; }

        IWorkflowProceduresList WorkflowProcedures { get; set; }

        T GetDependency<T>() where T : class;

        void SetDependencies(IDependencyContainer dc);

        bool CheckDependencies();
    }
}