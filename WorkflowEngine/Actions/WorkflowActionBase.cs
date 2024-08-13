using System;
using System.Linq;
using System.Xml.Linq;
using WorkflowEngine.Helpers.Audit;
using WorkflowEngine.Actions.Implementations;
using System.Threading.Tasks;
using WorkflowEngine.Core;
using WorkflowEngine.Helpers;

namespace WorkflowEngine.Actions
{
    public abstract class WorkflowActionBase
    {
        public int Depth { get; set; }
        public string Name => Item.GetAttribute("name", ContextData) ?? GetStaticName();

        private string GetStaticName()
        {
            if (SkipExecute)
                return GetType().Name;
            else
                return $"{GetType().Name}_{Item.GetHashValue()}";
        }

        // added .Name for testability
        public bool SkipExecute => new[] { typeof(UserTaskAction).Name, typeof(FinishProcess).Name }.Contains(GetType().Name);

        public Guid Id
        {
            get
            {
                bool v = Guid.TryParse(Item.GetAttribute("id", ContextData), out Guid result);
                return result;
            }
        }

        public XElement Item { get; internal set; }

        internal IInstance CurrentInstance { get; set; }

        internal IContextData ContextData => CurrentInstance.ContextData;

        public void Audit(string info = "", WorkflowAuditState state = WorkflowAuditState.Success)
        {
            CurrentInstance.AddAuditItem(new WorkflowAuditItem()
            {
                NodeId = Id,
                Name = Name,
                ActionType = GetType().Name.Replace("Action", ""),
                RequestId = CurrentInstance.ContextData.GetCurrentRequestId(),
                Info = info,
                State = state,
                Timestamp = DateTime.UtcNow,
                Depth = Depth
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
