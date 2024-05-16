using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowEngine.Core.Dependencies.WorkflowProcedures
{
    public interface IWorkflowProcedureService
    {
        Task<string> GetWorkflowProcedureAsync(string procedureName);
    }
}
