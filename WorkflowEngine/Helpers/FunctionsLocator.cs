using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowEngine.Core.Dependencies.CustomFunctions;
using WorkflowEngine.Core.Evaluation;

namespace WorkflowEngine.Helpers
{
    public class FunctionsLocator
    {
        IInstance _currentInstance;
        public FunctionsLocator(IInstance currentInstance)
        {
            _currentInstance = currentInstance;
        }

        public string Execute(string functionName, string args)
        {
            try
            {
                return InlineFunctions.Execute(functionName + "|" + args);
            }
            catch (InvalidOperationException)
            {
                var cfp = _currentInstance.GetDependency<ICustomFunctionProvider>();
                cfp.ContextData = _currentInstance.ContextData.Data.ToString();
                return cfp.Execute(functionName, args);
            }

        }
    }
}
