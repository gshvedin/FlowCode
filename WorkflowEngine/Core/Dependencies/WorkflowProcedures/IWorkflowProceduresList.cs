using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowEngine.Core.Evaluation;

namespace WorkflowEngine.Core.WorkflowProcedures
{
    public interface IWorkflowProceduresList
    {
         Task<string> GetWorkflowProcedure(string procedureName, IEnumerable<Parameter> parameters = null);
    }
}