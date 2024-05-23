using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using WorkflowEngine.Core;
using WorkflowEngine.Core.Dependencies.Strategies;
using WorkflowEngine.Core.Dependencies.WorkflowProcedures;
using WorkflowEngine.Core.Evaluation;
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
            string version = Item.GetAttribute("version");
            string versionType = Item.GetAttribute("versionType");
            List<Parameter> parameters = new List<Parameter>();
            if (!string.IsNullOrEmpty(version))
            {
                parameters.Add(new Parameter() { Name = "version", Value = version });
            }
            if (!string.IsNullOrEmpty(version))
            {
                parameters.Add(new Parameter("versionType", versionType));
            }

            string actionXml = await CurrentInstance.WorkflowProcedures.GetWorkflowProcedure(procedureName, parameters);

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
