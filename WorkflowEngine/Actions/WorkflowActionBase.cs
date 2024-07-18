using System;
using System.Linq;
using System.Xml.Linq;
using WorkflowEngine.Helpers.Audit;
using WorkflowEngine.Actions.Implementations;
using System.Threading.Tasks;

namespace WorkflowEngine.Actions
{
    public abstract class WorkflowActionBase
    {
   
        public string Name => Item?.Attribute("name")?.Value ?? GetType().Name;

        // added .Name for testability
        public bool SkipExecute => new[] { typeof(UserTaskAction).Name, typeof(FinishProcess).Name }.Contains(GetType().Name);

        public Guid Id
        {
            get
            {
                bool v = Guid.TryParse(Item?.Attribute("id")?.Value, out Guid result);
                return result;
            }
        }

        public XElement Item { get; internal set; }

        internal IInstance CurrentInstance { get; set; }

        public void Audit(string info = "", WorkflowAuditState state = WorkflowAuditState.Success)
        {
            CurrentInstance.AuditItems.Add(new WorkflowAuditItem()
            {
                NodeId = Id,
                NodeName = Name,
                RequestId = CurrentInstance.ContextData.GetCurrentRequestId(),
                Info = info,
                State = state,
                Timestamp = DateTime.UtcNow
            });
        }

        public void Execute()
        {
            _ = ExecuteAsync();
        }

        public virtual async Task ExecuteAsync()
        {
            await Task.Run(() =>
            {
                Audit();
            });
        }
    }
}
