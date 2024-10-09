using System.Threading.Tasks;
using System.Xml.Linq;
using WorkflowEngine.Core.Evaluation;
using WorkflowEngine.Helpers;

namespace WorkflowEngine.Actions.Implementations
{
    /// <summary>
    /// Stop process and run after external calling
    /// </summary>
    internal class UserTaskAction : WorkflowActionBase
    {
        public UserTaskAction(XElement item)
        {
            Item = item;
        }

        public override async Task ExecuteAsync()
        {
            Parameters = new Parameters().Read(Item, CurrentInstance);
            Keys["userTaskName"] = Item.GetAttribute("userTaskName", ContextData) ?? Item.GetAttribute("name", ContextData);
            Keys["output"] = Item.GetAttribute("output", ContextData);
            await Task.Run(() =>
            {
                Parameters = new Parameters().Read(Item, CurrentInstance);
                if (CurrentInstance.SaveTracking)
                    ContextData.SaveTracking(this);
                Audit();
            });
        }
    }
}