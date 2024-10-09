using System;
using System.Linq;
using System.Xml.Linq;
using WorkflowEngine.Helpers.Audit;
using WorkflowEngine.Actions.Implementations;
using System.Threading.Tasks;
using WorkflowEngine.Core;
using WorkflowEngine.Helpers;
using WorkflowEngine.Core.Evaluation;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace WorkflowEngine.Actions
{
    public abstract class WorkflowActionBase
    {
        internal Parameters Parameters { get; set; }
        internal Dictionary<string,string> Keys { get; set; } = new Dictionary<string,string>();



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
        public bool SkipExecute => new[] { typeof(UserTaskAction).Name, typeof(FinishProcess).Name, typeof(ExceptionAction).Name }.Contains(GetType().Name);

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



        internal JObject GetProcessInfo()
        {
            var currentProcessInfo = new JObject();
            // Add key-value pairs from the Keys dictionary
            currentProcessInfo["processName"] = Name;
            foreach (var kvp in Keys)
            {
                currentProcessInfo[kvp.Key] = kvp.Value;
            }
            // Add Parameters
            var parametersObj = new JObject();
            if (Parameters != null && Parameters.Any())
            {
                foreach (var param in Parameters)
                {
                    parametersObj[param.Name] = JToken.FromObject(param.Value);
                }
                currentProcessInfo["parameters"] = parametersObj;
            }
          

            return currentProcessInfo;
        }
    }
}
