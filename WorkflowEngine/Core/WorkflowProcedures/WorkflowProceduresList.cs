using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using WorkflowEngine.Core.Dependencies;
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
                Add(new WorkflowProcedureItem() { Name = xe.GetAttribute("name"), Definition = xe.ToString() });
            }
        }

        public string GetWorkflowProcedure(string procedureName)
        {
            return this.FirstOrDefault(i => i.Name == procedureName)?.Definition ??
              currentInstance.GetDependency<IWorkflowProcedureContainer>()?.GetWorkflowProcedure(procedureName);
        }
    }
}
