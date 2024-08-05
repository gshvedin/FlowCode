using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using WorkflowEngine.Core;
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
            string procedureName = Item.GetAttribute("procedureName", ContextData);

            if (string.IsNullOrEmpty(procedureName))
            {
                throw new WorkflowException("Property procedureName is not defined");
            }

            string version = Item.GetAttribute("version", ContextData);
            string versionType = Item.GetAttribute("versionType", ContextData) ?? Item.GetAttribute("matchVer", ContextData);
            List<Parameter> parameters = new List<Parameter>();
            if (!string.IsNullOrEmpty(version))
            {
                parameters.Add(new Parameter("version", version));
            }
            if (!string.IsNullOrEmpty(versionType))
            {
                parameters.Add(new Parameter("versionType", versionType));
            }

            string actionXml = await CurrentInstance.WorkflowProcedures.GetWorkflowProcedure(procedureName, parameters);

            if (!string.IsNullOrEmpty(actionXml))
            {
                Parameters procedureParams = new Parameters().Read(Item, CurrentInstance);

                foreach (var param in procedureParams)
                {
                    //CurrentInstance.ContextData.SetValue(param.Name, param.Value); REMOVE
                    CurrentInstance.ContextData.SetArgument(param.Name, param.Value?.ToString());
                }


                await new WorkflowContext(CurrentInstance, actionXml, Depth + 1).ExecuteAsync();

                foreach (var param in procedureParams)
                {
                    CurrentInstance.ContextData.RemoveValue(param.Name);
                }

            }
            else
            {
                throw new WorkflowException($"Workflow procedure '{procedureName}' was not resolved");
            }

            Audit();
        }
    }
}