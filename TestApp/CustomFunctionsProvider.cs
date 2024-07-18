using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowEngine.Core.Dependencies.CustomFunctions;

namespace TestApp
{
    internal class CustomFunctionsProvider : ICustomFunctionProvider
    {
        public string execute(string functionName, string args)
        {
            switch (functionName)
            {
                case "getMessageLang":
                    return getMessageLang(args);
                // Add more cases for other functions
                case "getCurrencyName":
                    return getCurrencyName(args);
                // Add more cases for other functions
                default:
                    throw new InvalidOperationException($"Function '{functionName}' not found.");
            }
        }

        private string getMessageLang(string args)
        {
            string key = args.Split('|')[0];
            string scope = args.Split('|')[1];
            string locale = args.Split('|')[2];
            // Your logic to get the locale value
            // get locale from db
            return $"{key}:localized_value";
        }

        private string getCurrencyName(string args)
        {
            string currTerminalCurrency = "";// get from session
            var result = "";
            return result;
        }

        // Add more functions as needed
    }
}
