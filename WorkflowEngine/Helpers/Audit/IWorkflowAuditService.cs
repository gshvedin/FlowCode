using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WorkflowEngine.Helpers.Audit
{
    public interface IWorkflowAuditService
    {
        Task AddAuditItem(WorkflowAuditItem item);

        Task AddAuditItems(IList<WorkflowAuditItem> auditItems, Dictionary<string, object> metaInfo);

        Task<IEnumerable<WorkflowAuditItem>> GetAudit(Guid requestId);
    }
}
