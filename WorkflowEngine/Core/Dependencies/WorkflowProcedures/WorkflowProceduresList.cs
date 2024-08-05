using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using WorkflowEngine.Core.Dependencies;
using WorkflowEngine.Core.Dependencies.WorkflowProcedures;
using WorkflowEngine.Core.Evaluation;
using WorkflowEngine.Helpers;

namespace WorkflowEngine.Core.WorkflowProcedures
{
    public class WorkflowProceduresList : List<WorkflowProcedureItem>, IWorkflowProceduresList
    {
        private IInstance currentInstance;

        public WorkflowProceduresList(string workflowDefinition, IInstance currentInstance)
        {
            this.currentInstance = currentInstance;
            if (workflowDefinition == null)
            {
                return;
            }

            XDocument wfXml = XDocument.Parse(workflowDefinition);
            foreach (XElement xe in wfXml.Root.Elements("WorkflowProcedures")?.Elements())
            {
                Add(new WorkflowProcedureItem() { Name = xe.GetAttribute("name", currentInstance.ContextData), Definition = xe.ToString() });
            }
        }

        public async Task<string> GetWorkflowProcedure(string procedureName, IEnumerable<Parameter> parameters = null)
        {
            if (this.FirstOrDefault(i => i.Name == procedureName)?.Definition is string inlineWfp && !string.IsNullOrEmpty(inlineWfp))
            {
                return await Task.FromResult(inlineWfp);
            }

            return await currentInstance.GetDependency<IWorkflowProcedureService>()?.GetWorkflowProcedureAsync(procedureName, parameters);
        }
    }
}
