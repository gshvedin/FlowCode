using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowEngine.Core.Dependencies.CustomFunctions
{
    public interface ICustomFunctionProvider
    {
        string execute(string functionName, string args);
    }
}
