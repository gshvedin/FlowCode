using System.Collections.Generic;
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
            string procedureName = Item.GetAttribute("procedureName");

            if (string.IsNullOrEmpty(procedureName))
            {
                throw new WorkflowException("Property procedureName is not defined");
            }

            string version = Item.GetAttribute("version");
            string versionType = Item.GetAttribute("versionType") ?? Item.GetAttribute("matchVer");
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
                Parameters procedureParams = new Parameters().Read(Item, CurrentInstance);

                foreach (var param in procedureParams)
                {
                    CurrentInstance.ContextData.SetValue(param.Name, param.Value);
                }


                await new WorkflowContext(CurrentInstance, actionXml).ExecuteAsync();

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