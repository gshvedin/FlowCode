using System.Globalization;
using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using WorkflowEngine.Core.Evaluation;
using WorkflowEngine.Helpers;

namespace WorkflowEngine.Actions.Implementations
{
    /// <summary>
    /// Save result of executed expression to defined path of appData
    /// </summary>
    internal class DataRemoveAction : WorkflowActionBase
    {
        public DataRemoveAction(XElement item)
        {
            Item = item;
        }

        public override async Task ExecuteAsync()
        {
            await Task.Run(Execute);
            Audit();
        }

        public new void Execute()
        {
            string savingPath = Item.GetAttribute("output", ContextData) ?? Name;
            string saveAs = Item.GetAttribute("saveAs", ContextData)?.ToLower(CultureInfo.CurrentCulture) ?? "string";

            if (saveAs.StartsWith("arg", StringComparison.InvariantCulture))
            {
                CurrentInstance.ContextData.RemoveArgument(savingPath);
            }
            else
            {
                CurrentInstance.ContextData.RemoveValue(savingPath);
            }
          
            Audit();
        }
    }
}