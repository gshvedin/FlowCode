﻿using Jint;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Xml.Linq;
using WorkflowEngine.Core.Evaluation;
using WorkflowEngine.Helpers;

namespace WorkflowEngine.Actions.Implementations
{
    internal class ScriptAction : WorkflowActionBase
    {
        public ScriptAction(XElement item)
        {
            Item = item;
        }

        public override async Task ExecuteAsync()
        {
            await Task.Run(() =>
               {
                   DateTime dateStart = DateTime.Now;
                   string script = Item.Element("ScriptText").Value.Trim();
                   string savingPath = Item.GetAttribute("output", ContextData) ?? Name;
                   Parameters parameters = new Parameters().Read(Item, CurrentInstance);
                   string input = CurrentInstance.ContextData.GetValue(Item.GetAttribute("path", ContextData));

                   string result = ExecuteJavaScript(script, input, parameters);


                   if (Item.GetAttribute("saveAs", ContextData)?.ToLower(CultureInfo.CurrentCulture)?.StartsWith("j", StringComparison.InvariantCulture) ?? false)
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

        public string ExecuteJavaScript(string script, string jsonData, Parameters parameters)
        {
            var engine = new Engine(cfg => cfg.AllowClr(typeof(FunctionsLocator).Assembly));
            var fl = new FunctionsLocator(CurrentInstance);
            engine.SetValue("fn_execute", new Func<string, string, string>(fl.Execute));
            engine.SetValue("fn_genID", new Func<string>(ScriptExtensions.GenerateGuid));
            engine.SetValue("jpath_query", new Func<string, string, string>(ScriptExtensions.ExecuteJPathQuery));

            // Set JSON data in the JavaScript environment
            engine.SetValue("context", jsonData);

            // Add parameters to the JavaScript environment
            foreach (var parameter in parameters)
            {
                engine.SetValue(parameter.Name, parameter.Value);
            }

            // Execute the JavaScript script
            var result = engine.Execute(script).GetValue("result");
            return result.ToString();
        }

    }
}