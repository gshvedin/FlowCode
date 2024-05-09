using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WorkflowEngine.Core;
using WorkflowEngine.Core.Dependencies;
using WorkflowEngine.Core.WorkflowProcedures;
using WorkflowEngine.Helpers.Audit;
using WorkflowEngine.Misc;

namespace WorkflowEngine
{
    public class Workflow
    {
        public Workflow(string workflowDefinition, IDependencyContainer container = null)
        {
            CurrentInstance.WorkflowDefinition = workflowDefinition;
            CurrentInstance.SetDependencies(container);
            CurrentInstance.WorkflowProcedures = new WorkflowProceduresList(workflowDefinition, CurrentInstance);
        }

        public Workflow(string workflowDefinition, IInstance instance = null)
        {
            CurrentInstance = instance;
            instance.WorkflowDefinition = workflowDefinition;
            CurrentInstance.WorkflowProcedures = new WorkflowProceduresList(workflowDefinition, CurrentInstance);
        }

        public IInstance CurrentInstance { get; set; } = new Instance();

        public IWorkflowContext WorkflowContext { get; set; }

        public void SetDependencies(IDependencyContainer container)
        {
            CurrentInstance.SetDependencies(container);
        }

        public bool SetMetaInfo(Dictionary<string, object> metaInfo)
        {
            if (CurrentInstance?.DC == null || metaInfo == null)
            {
                return false;
            }

            CurrentInstance.DC.MetaInfo = metaInfo;
            return true;
        }

        public bool CheckDependencies()
        {
            return CurrentInstance.CheckDependencies();
        }

        public async Task<string> ExecuteAsync(string contextData, CancellationToken cancellationToken = default)
        {
            CurrentInstance.ContextData = new ContextData(contextData, CurrentInstance);
            CurrentInstance.CancellationToken = cancellationToken;

            if (string.IsNullOrEmpty(CurrentInstance.WorkflowDefinition))
            {
                throw new WorkflowException("WorkflowDefinition is not set");
            }

            try
            {
                WorkflowContext = await (WorkflowContext ?? new WorkflowContext(CurrentInstance)).ExecuteAsync();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            finally
            {
                IWorkflowAuditService was = CurrentInstance.GetDependency<IWorkflowAuditService>();
                if (was != null)
                {
                    await was?.AddAuditItems(CurrentInstance.AuditItems, CurrentInstance.DC.MetaInfo);
                }
            }

            return CurrentInstance.ContextData.Data.ToString();
        }
    }
}
