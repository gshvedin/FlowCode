using System.Globalization;
using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using WorkflowEngine.Core.Dependencies.Connectors;
using WorkflowEngine.Core.Evaluation;
using WorkflowEngine.Helpers;
using WorkflowEngine.Misc;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using Newtonsoft.Json;

namespace WorkflowEngine.Actions.Implementations
{
    internal class ScriptAction : WorkflowActionBase
    {
        public ScriptAction(XElement item) : base()
        {
            Item = item;
        }

        public override async Task ExecuteAsync()
        {
            await Task.Run(() =>
               {
                   DateTime dateStart = DateTime.Now;
                   string script = Item.Element("ScriptText").Value.Trim();
                   string savingPath = Item?.Attribute("output")?.Value ?? Name;
                   Parameters parameters = new Parameters().Read(Item, CurrentInstance);
                   string input = CurrentInstance.ContextData.GetValue(Item?.Attribute("path")?.Value);
                   string result = ExecutePythonScript(script, input, parameters);

                   if (Item.GetAttribute("saveAs")?.ToLower(CultureInfo.CurrentCulture)?.StartsWith("j", StringComparison.InvariantCulture) ?? false)
                   {
                       CurrentInstance.ContextData.SetValueAsJsonNode(savingPath, result);
                   }
                   else
                   {
                       CurrentInstance.ContextData.SetValue(savingPath, result);
                   }

                   Audit($"Execution duration {DateTime.Now.Subtract(dateStart).TotalMilliseconds} ms");
               });


        }

        private string ExecutePythonScript(string script, string data, Parameters parameters)
        {
            Console.WriteLine($"f1:{DateTime.Now}");
            // Set up IronPython engine
            ScriptEngine engine = Python.CreateEngine();
            ScriptScope scope = engine.CreateScope();
            Console.WriteLine($"f2:{DateTime.Now}");
            // Pass JSON data to the Python script via scope
            scope.SetVariable("context", data);

            //add parameters to context
            foreach (var parameter in parameters)
            {
                scope.SetVariable(parameter.Name, parameter.Value);
            }

            // Execute the Python script
            dynamic result = engine.Execute(script, scope);
            Console.WriteLine($"f3:{DateTime.Now}");
            return result;
        }
    }
}