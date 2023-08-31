using System.Threading.Tasks;
using System.Xml.Linq;
using WorkflowEngine.Core;
using WorkflowEngine.Helpers;
using WorkflowEngine.Misc;

namespace WorkflowEngine.Actions.Implementations
{
    public class WorkflowProcedureAction : WorkflowActionBase
    {
        public WorkflowProcedureAction(XElement item)
        {
            Item = item;
        }

        [MethodTimer.Time]
        public override async Task ExecuteAsync()
        {
            string procedureName = Item.GetAttribute("procedureName");
            string actionXml = CurrentInstance.WorkflowProcedures.GetWorkflowProcedure(procedureName);

            if (!string.IsNullOrEmpty(actionXml))
            {
                await new WorkflowContext(CurrentInstance, actionXml).ExecuteAsync();
            }
            else
            {
                throw new WorkflowException($"Workflow procedure '{procedureName}' was not resolved");
            }

            Audit();
        }
    }
}
