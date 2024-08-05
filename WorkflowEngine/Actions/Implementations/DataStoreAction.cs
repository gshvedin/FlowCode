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
    internal class DataStoreAction : WorkflowActionBase
    {
        public DataStoreAction(XElement item)
        {
            Item = item;
        }

        public override async Task ExecuteAsync()
        {
            string result = await new EvaluateBase(Item, CurrentInstance).EvaluateAsync();
            string savingPath = Item.GetAttribute("output", ContextData) ?? Name;
            string saveAs = Item.GetAttribute("saveAs", ContextData)?.ToLower(CultureInfo.CurrentCulture) ?? "string";

            if (saveAs.StartsWith("arg", StringComparison.InvariantCulture))
            {
                CurrentInstance.ContextData.SetArgument(savingPath, result);
            }
            else if (saveAs.StartsWith("j", StringComparison.InvariantCulture))
            {
                CurrentInstance.ContextData.SetValueAsJsonNode(savingPath, result);
            }
            else
            {
                CurrentInstance.ContextData.SetValue(savingPath, result);
            }

            Audit();
        }
    }
}