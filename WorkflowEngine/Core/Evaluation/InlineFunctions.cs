using Esprima.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowEngine.Core.Evaluation
{
    public static class InlineFunctions
    {

        public static string Execute(string args)
        {
            var argsArray = args.Split('|');
            // replace fn_ in begining to empty 
            string functionName = argsArray[0]?.Replace("fn_", "")?.ToLower();
         
            switch (functionName)
            {
                case "genid":
                    return Helpers.ScriptExtensions.GenerateGuid();
                // Add more cases for other functions
                case "getrandom":
                    {
                        if(argsArray.Length < 2)
                        {
                            throw new InvalidOperationException("Function 'fn_getRandom' requires at least one argument.");
                        }
                        if (string.IsNullOrEmpty(argsArray[1]))
                        {
                            throw new InvalidOperationException("Function 'fn_getRandom' requires a non-empty argument.");
                        }
                        return RandomExtensions.GetRandom(argsArray[1]);
                    }
                case "currentdate":
                    {
                        if (argsArray.Length > 1)
                        {
                            return DateTime.Now.ToString(argsArray[1]);
                        }
                         return DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                    }
                    
                case "unixtime":
                    return DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                // Add more cases for other functions
                default:
                    throw new InvalidOperationException($"Function '{functionName}' not found.");
            }
        }
    }
}
