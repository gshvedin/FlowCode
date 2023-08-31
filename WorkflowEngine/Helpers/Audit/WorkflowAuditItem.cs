using Newtonsoft.Json;
using System;

namespace WorkflowEngine.Helpers.Audit
{
    public class WorkflowAuditItem
    {
        public long AuditId { get; set; }

        public Guid RequestId { get; set; }

        public Guid NodeId { get; set; }

        public string NodeName { get; set; }

        public DateTime Timestamp { get; set; }

        public string Info { get; set; }

        public WorkflowAuditState State { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}