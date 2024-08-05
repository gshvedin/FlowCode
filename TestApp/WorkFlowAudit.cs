using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// using System.Threading;
using WorkflowEngine.Helpers.Audit;

namespace TestApp
{
#pragma warning disable SA1402 // File may only contain a single type
    public class WorkFlowAudit : IWorkflowAuditService
#pragma warning restore SA1402 // File may only contain a single type
    {
        public Task AddAuditItem(WorkflowAuditItem item)
        {
          //  Console.WriteLine(item.Info);
            return Task.CompletedTask;
        }

        public Task AddAuditItems(IList<WorkflowAuditItem> auditItems, Dictionary<string, object> metaInfo)
        {
            //foreach (var item in auditItems)
            //    Console.WriteLine(item.Info);

            return Task.CompletedTask;
        }

        public Task<IEnumerable<WorkflowAuditItem>> GetAudit(Guid requestId)
        {
            throw new NotImplementedException();
        }
    }



 

}
