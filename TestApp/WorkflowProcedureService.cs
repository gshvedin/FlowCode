using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

// using System.Threading;
using WorkflowEngine.Core.Dependencies.Strategies;
using WorkflowEngine.Core.Dependencies.WorkflowProcedures;
using WorkflowEngine.Core.Evaluation;

namespace TestApp
{
    public class WorkflowProcedureService : IWorkflowProcedureService
    {
  
        public Task<string> GetWorkflowProcedureAsync(string procedureName, IEnumerable<Parameter> parameters = null)
        {
            try
            {
                string versionValue = parameters.FirstOrDefault(p => p.Name == "version")?.Value.ToString();
                string versionTypeValue = parameters.FirstOrDefault(p => p.Name == "versionType")?.Value.ToString();

                string wfData = System.IO.File.ReadAllText("TestData/WFProcedures/" + procedureName + ".xml");
                return Task.FromResult(wfData);
            }
            catch (FileNotFoundException ex)
            {

                throw new System.Exception("Workflow procedure with name {procedureName}.xml not found in /WFProcedures directory");
            }
            catch (System.Exception ex)
            {
                throw;
            }



        }
 
    }





}
