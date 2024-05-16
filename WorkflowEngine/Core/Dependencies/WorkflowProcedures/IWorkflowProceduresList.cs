using System.Threading.Tasks;

namespace WorkflowEngine.Core.WorkflowProcedures
{
    public interface IWorkflowProceduresList
    {
         Task<string> GetWorkflowProcedure(string procedureName);
    }
}